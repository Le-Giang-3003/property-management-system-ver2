using PropertyManagementSystemVer2.BLL.DTOs.Auth;
using PropertyManagementSystemVer2.BLL.Mapping;
using PropertyManagementSystemVer2.BLL.Security;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;
using System.Text.RegularExpressions;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPasswordValidator _passwordValidator;
        private readonly IEmailService _emailService;
        private readonly IOtpGenerator _otpGenerator;
        private readonly ISmsService _smsService;

        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;
        private const int OtpExpiryMinutes = 30;
        private const int EmailVerifyExpiryHours = 24;

        public AuthService(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, IPasswordValidator passwordValidator, IEmailService emailService, IOtpGenerator otpGenerator, ISmsService smsService)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _passwordValidator = passwordValidator;
            _emailService = emailService;
            _otpGenerator = otpGenerator;
            _smsService = smsService;
        }

        public async Task<AuthResultDto> LoginAsync(LoginRequestDto request, string? ipAddress = null)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

            if (user == null)
                return AuthResultDto.Fail("Email hoặc mật khẩu không đúng.");

            if (!user.IsActive)
                return AuthResultDto.Fail("Tài khoản đã bị vô hiệu hóa.");

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                var remaining = (user.LockoutEnd.Value - DateTime.UtcNow).Minutes + 1;
                return AuthResultDto.Fail($"Tài khoản bị khóa. Vui lòng thử lại sau {remaining} phút.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                    user.FailedLoginAttempts = 0;
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                    return AuthResultDto.Fail($"Đăng nhập sai {MaxFailedAttempts} lần. Tài khoản bị khóa {LockoutMinutes} phút.");
                }

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return AuthResultDto.Fail("Email hoặc mật khẩu không đúng.");
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress
            });

            await _unitOfWork.SaveChangesAsync();

            return AuthResultDto.Ok("Đăng nhập thành công.", new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(60),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
                User = UserMapper.ToDto(user),
            });
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
        {
            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);

            if (storedToken == null || !storedToken.IsActive)
                return AuthResultDto.Fail("Refresh token không hợp lệ hoặc đã hết hạn.");

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = ipAddress;
            _unitOfWork.RefreshTokens.Update(storedToken);

            var user = await _unitOfWork.Users.GetByIdAsync(storedToken.UserId);
            if (user == null || !user.IsActive)
                return AuthResultDto.Fail("Tài khoản không hợp lệ.");

            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress
            });

            await _unitOfWork.SaveChangesAsync();

            return AuthResultDto.Ok(data: new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(60),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
                User = UserMapper.ToDto(user),
            });
        }

        public async Task<AuthResultDto> RevokeRefreshTokenAsync(string refreshToken, string? ipAddress = null)
        {
            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);

            if (storedToken == null || !storedToken.IsActive)
                return AuthResultDto.Fail("Token không hợp lệ.");

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = ipAddress;
            _unitOfWork.RefreshTokens.Update(storedToken);
            await _unitOfWork.SaveChangesAsync();

            return AuthResultDto.Ok("Token đã bị thu hồi.");
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterRequestDto request)
        {
            var passwordErrors = _passwordValidator.Validate(request.Password);
            if (passwordErrors.Count > 0)
                return AuthResultDto.Fail("Mật khẩu không đủ mạnh: " + string.Join("; ", passwordErrors));

            if (request.Password != request.ConfirmPassword)
                return AuthResultDto.Fail("Mật khẩu xác nhận không khớp.");

            var email = request.Email.Trim().ToLower();

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return AuthResultDto.Fail("Email không hợp lệ.");

            if (await _unitOfWork.Users.EmailExistsAsync(email))
                return AuthResultDto.Fail("Email đã được đăng ký.");

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Address = request.Address,
                Role = UserRole.Member,
                IsTenant = true,
                IsLandlord = false,
                IsActive = true,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Email verification token (24h)
            var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            await _unitOfWork.EmailVerificationTokens.AddAsync(new EmailVerificationToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(EmailVerifyExpiryHours)
            });
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, token);
            }
            catch (Exception)
            {
                return AuthResultDto.Ok(
                    "Đăng ký thành công. Gửi email xác thực thất bại, vui lòng dùng chức năng gửi lại.",
                    new RegisterResponseDto
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        Message = "Gửi email thất bại. Bạn có thể yêu cầu gửi lại email xác thực."
                    });
            }

            return AuthResultDto.Ok("Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản.",
                new RegisterResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Message = $"Email xác thực đã được gửi tới {user.Email}. Có hiệu lực trong {EmailVerifyExpiryHours}h."
                });
        }

        public async Task<AuthResultDto> VerifyEmailAsync(string token)
        {
            var verification = await _unitOfWork.EmailVerificationTokens.GetValidTokenAsync(token);

            if (verification == null)
                return AuthResultDto.Fail("Link xác thực không hợp lệ hoặc đã hết hạn.");

            verification.IsUsed = true;
            verification.UsedAt = DateTime.UtcNow;
            _unitOfWork.EmailVerificationTokens.Update(verification);

            var user = await _unitOfWork.Users.GetByIdAsync(verification.UserId);
            if (user != null)
            {
                user.IsEmailVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
            }

            await _unitOfWork.SaveChangesAsync();
            return AuthResultDto.Ok("Xác thực email thành công.");
        }

        public async Task<AuthResultDto> ResendEmailVerificationAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);

            if (user == null)
                return AuthResultDto.Fail("Email không tồn tại.");

            if (user.IsEmailVerified)
                return AuthResultDto.Fail("Email đã được xác thực.");

            // Invalidate token cũ
            await _unitOfWork.EmailVerificationTokens.InvalidateAllByUserAsync(user.Id);

            // Tạo token mới
            var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            await _unitOfWork.EmailVerificationTokens.AddAsync(new EmailVerificationToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(EmailVerifyExpiryHours)
            });
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, token);

            return AuthResultDto.Ok($"Email xác thực đã được gửi lại tới {user.Email}.");
        }

        public async Task<AuthResultDto> LogoutAsync(int userId, string refreshToken, string? ipAddress = null)
        {
            await RevokeRefreshTokenAsync(refreshToken, ipAddress);
            return AuthResultDto.Ok("Đăng xuất thành công.");
        }

        public async Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var identifier = request.Identifier.Trim();
            var user = await _unitOfWork.Users.GetByEmailOrPhoneAsync(identifier);

            if (user == null)
                return AuthResultDto.Ok("Nếu tài khoản tồn tại, OTP đã được gửi.");

            var otp = _otpGenerator.Generate();

            await _unitOfWork.OtpVerifications.AddAsync(new OtpVerification
            {
                UserId = user.Id,
                Identifier = identifier,
                OtpCode = otp,
                Purpose = "ResetPassword",
                ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes)
            });
            await _unitOfWork.SaveChangesAsync();

            try
            {
                if (identifier.Contains('@'))
                    await _emailService.SendPasswordResetOtpAsync(user.Email, user.FullName, otp);
                else
                    await _smsService.SendOtpAsync(identifier, otp);
            }
            catch (Exception)
            {
                return AuthResultDto.Fail("Gửi OTP thất bại. Vui lòng thử lại sau.");
            }

            var channel = identifier.Contains('@') ? "email" : "SĐT";
            return AuthResultDto.Ok($"OTP đã được gửi qua {channel}. Có hiệu lực 30 phút, chỉ dùng 1 lần.");
        }

        public async Task<AuthResultDto> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var passwordErrors = _passwordValidator.Validate(request.NewPassword);
            if (passwordErrors.Count > 0)
                return AuthResultDto.Fail("Mật khẩu không đủ mạnh: " + string.Join("; ", passwordErrors));

            if (request.NewPassword != request.ConfirmPassword)
                return AuthResultDto.Fail("Mật khẩu xác nhận không khớp.");

            var otp = await _unitOfWork.OtpVerifications.GetValidOtpAsync(request.Token, "ResetPassword");

            if (otp == null)
                return AuthResultDto.Fail("OTP không hợp lệ hoặc đã hết hạn.");

            otp.IsUsed = true;
            otp.UsedAt = DateTime.UtcNow;
            _unitOfWork.OtpVerifications.Update(otp);

            var user = await _unitOfWork.Users.GetByIdAsync(otp.UserId!.Value);
            if (user == null)
                return AuthResultDto.Fail("Người dùng không tồn tại.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);

            // Invalidate tất cả refresh tokens
            await _unitOfWork.RefreshTokens.RevokeAllByUserAsync(user.Id);

            await _unitOfWork.SaveChangesAsync();
            return AuthResultDto.Ok("Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại.");
        }

        public async Task<AuthResultDto> GetProfileAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return AuthResultDto.Fail("Người dùng không tồn tại.");

            return AuthResultDto.Ok(data: UserMapper.ToDto(user));
        }

        public async Task<AuthResultDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return AuthResultDto.Fail("Người dùng không tồn tại.");

            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
            {
                user.PhoneNumber = request.PhoneNumber.Trim();
                user.IsPhoneVerified = false;
            }

            if (request.Address != null)
                user.Address = request.Address.Trim();

            if (request.AvatarUrl != null)
                user.AvatarUrl = request.AvatarUrl;

            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return AuthResultDto.Ok("Cập nhật thông tin thành công.", UserMapper.ToDto(user));
        }

        public async Task<AuthResultDto> UpdateLandlordInfoAsync(int userId, UpdateLandlordInfoRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return AuthResultDto.Fail("Người dùng không tồn tại.");

            if (!user.IsLandlord)
                return AuthResultDto.Fail("Bạn cần được cấp quyền Landlord trước.");

            user.IdentityNumber = request.IdentityNumber.Trim();
            user.BankAccountNumber = request.BankAccountNumber.Trim();
            user.BankName = request.BankName.Trim();
            user.BankAccountHolder = request.BankAccountHolder.Trim();
            user.IsIdentityVerified = true;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return AuthResultDto.Ok("Cập nhật thông tin Landlord thành công.", UserMapper.ToDto(user));
        }

        public async Task<AuthResultDto> SendPhoneOtpAsync(int userId, SendPhoneOtpRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return AuthResultDto.Fail("Người dùng không tồn tại.");

            var otp = _otpGenerator.Generate();
            await _unitOfWork.OtpVerifications.AddAsync(new OtpVerification
            {
                UserId = userId,
                Identifier = request.PhoneNumber.Trim(),
                OtpCode = otp,
                Purpose = "PhoneVerify",
                ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes)
            });
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await _smsService.SendOtpAsync(request.PhoneNumber.Trim(), otp);
            }
            catch (Exception)
            {
                return AuthResultDto.Fail("Gửi OTP qua SMS thất bại. Vui lòng thử lại sau.");
            }

            return AuthResultDto.Ok("OTP đã được gửi tới SĐT của bạn.");
        }

        public async Task<AuthResultDto> VerifyPhoneOtpAsync(int userId, VerifyPhoneOtpRequestDto request)
        {
            var otp = await _unitOfWork.OtpVerifications.GetValidPhoneOtpAsync(
                userId, request.PhoneNumber, request.OtpCode);

            if (otp == null)
                return AuthResultDto.Fail("OTP không hợp lệ hoặc đã hết hạn.");

            otp.IsUsed = true;
            otp.UsedAt = DateTime.UtcNow;
            _unitOfWork.OtpVerifications.Update(otp);

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user != null)
            {
                user.IsPhoneVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
            }   

            await _unitOfWork.SaveChangesAsync();
            return AuthResultDto.Ok("Xác thực SĐT thành công.");
        }
    }
}
