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

        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;
        private const int OtpExpiryMinutes = 30;
        private const int EmailVerifyExpiryHours = 24;

        public AuthService(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, IPasswordValidator passwordValidator, IEmailService emailService, IOtpGenerator otpGenerator)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _passwordValidator = passwordValidator;
            _emailService = emailService;
            _otpGenerator = otpGenerator;
        }

        public async Task<AuthResultDto> LoginAsync(LoginRequestDto request, string? ipAddress = null)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.FirstOrDefaultAsync(u => u.Email == request.Email.Trim().ToLower());

            if (user == null)
                return AuthResultDto.Fail("Email hoặc mật khẩu không đúng.");

            if (!user.IsActive)
                return AuthResultDto.Fail("Tài khoản đã bị vô hiệu hóa.");

            // Check lockout
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                var remaining = (user.LockoutEnd.Value - DateTime.UtcNow).Minutes + 1;
                return AuthResultDto.Fail($"Tài khoản bị khóa. Vui lòng thử lại sau {remaining} phút.");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                    user.FailedLoginAttempts = 0;
                    userRepo.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                    return AuthResultDto.Fail($"Đăng nhập sai {MaxFailedAttempts} lần. Tài khoản bị khóa {LockoutMinutes} phút.");
                }

                userRepo.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return AuthResultDto.Fail("Email hoặc mật khẩu không đúng.");
            }

            // Reset lockout on success
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;
            userRepo.Update(user);

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Save refresh token
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken>();
            await refreshTokenRepo.AddAsync(new RefreshToken
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
            var tokenRepo = _unitOfWork.GetRepository<RefreshToken>();
            var storedToken = await tokenRepo.FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null || !storedToken.IsActive)
                return AuthResultDto.Fail("Refresh token không hợp lệ hoặc đã hết hạn.");

            // Revoke old token
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = ipAddress;
            tokenRepo.Update(storedToken);

            // Get user
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(storedToken.UserId);
            if (user == null || !user.IsActive)
                return AuthResultDto.Fail("Tài khoản không hợp lệ.");

            // Generate new tokens
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            await tokenRepo.AddAsync(new RefreshToken
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
            var tokenRepo = _unitOfWork.GetRepository<RefreshToken>();
            var storedToken = await tokenRepo.FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null || !storedToken.IsActive)
                return AuthResultDto.Fail("Token không hợp lệ.");

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = ipAddress;
            tokenRepo.Update(storedToken);
            await _unitOfWork.SaveChangesAsync();

            return AuthResultDto.Ok("Token đã bị thu hồi.");
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Validate password: >= 8 ký tự, uppercase, lowercase, number, special char
            var passwordErrors = _passwordValidator.Validate(request.Password);
            if (passwordErrors.Count > 0)
                return AuthResultDto.Fail("Mật khẩu không đủ mạnh: " + string.Join("; ", passwordErrors));

            // 2. Confirm password
            if (request.Password != request.ConfirmPassword)
                return AuthResultDto.Fail("Mật khẩu xác nhận không khớp.");

            var userRepo = _unitOfWork.GetRepository<User>();
            var email = request.Email.Trim().ToLower();

            // 3. Validate email format
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return AuthResultDto.Fail("Email không hợp lệ.");

            // 4. Check duplicate email
            if (await userRepo.AnyAsync(u => u.Email == email))
                return AuthResultDto.Fail("Email đã được đăng ký.");

            // 5. Create user: Role = Member, IsTenant = true (default)
            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Address = request.Address,
                Role = UserRole.Member,      // Role mặc định: Member
                IsTenant = true,              // Mặc định là Tenant
                IsLandlord = false,
                IsActive = true,
                IsEmailVerified = false,      // Chưa xác thực email
                CreatedAt = DateTime.UtcNow
            };

            await userRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // 6. Generate email verification token (hiệu lực 24h)
            var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            var tokenRepo = _unitOfWork.GetRepository<EmailVerificationToken>();
            await tokenRepo.AddAsync(new EmailVerificationToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(EmailVerifyExpiryHours)
            });
            await _unitOfWork.SaveChangesAsync();

            // 7. GỬI EMAIL XÁC THỰC (real email via SMTP)
            try
            {
                await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, token);
            }
            catch (Exception)
            {
                // Log error nhưng vẫn return success (user có thể resend sau)
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
            var tokenRepo = _unitOfWork.GetRepository<EmailVerificationToken>();
            var verification = await tokenRepo.FirstOrDefaultAsync(t =>
                t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);

            if (verification == null)
                return AuthResultDto.Fail("Link xác thực không hợp lệ hoặc đã hết hạn.");

            verification.IsUsed = true;
            verification.UsedAt = DateTime.UtcNow;
            tokenRepo.Update(verification);

            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(verification.UserId);
            if (user != null)
            {
                user.IsEmailVerified = true;
                userRepo.Update(user);
            }

            await _unitOfWork.SaveChangesAsync();
            return AuthResultDto.Ok("Xác thực email thành công.");
        }

        public async Task<AuthResultDto> ResendEmailVerificationAsync(string email)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower());

            if (user == null)
                return AuthResultDto.Fail("Email không tồn tại.");

            if (user.IsEmailVerified)
                return AuthResultDto.Fail("Email đã được xác thực.");

            var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            var tokenRepo = _unitOfWork.GetRepository<EmailVerificationToken>();
            await tokenRepo.AddAsync(new EmailVerificationToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(EmailVerifyExpiryHours)
            });
            await _unitOfWork.SaveChangesAsync();

            // TODO: Send email
            return AuthResultDto.Ok("Email xác thực đã được gửi lại.");
        }

        public async Task<AuthResultDto> LogoutAsync(int userId, string refreshToken, string? ipAddress = null)
        {
            await RevokeRefreshTokenAsync(refreshToken, ipAddress);
            return AuthResultDto.Ok("Đăng xuất thành công.");
        }

        public async Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var identifier = request.Identifier.Trim();

            // Tìm user theo email hoặc SĐT
            var user = await userRepo.FirstOrDefaultAsync(u =>
                u.Email == identifier.ToLower() || u.PhoneNumber == identifier);

            if (user == null)
                return AuthResultDto.Ok("Nếu tài khoản tồn tại, OTP đã được gửi."); // Không tiết lộ user có tồn tại hay không

            // Generate OTP (6 digits)
            var otp = _otpGenerator.Generate();

            var otpRepo = _unitOfWork.GetRepository<OtpVerification>();
            await otpRepo.AddAsync(new OtpVerification
            {
                UserId = user.Id,
                Identifier = identifier,
                OtpCode = otp,
                Purpose = "ResetPassword",
                ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes)
            });
            await _unitOfWork.SaveChangesAsync();

            // Gửi OTP qua email
            try
            {
                if (identifier.Contains('@'))
                {
                    await _emailService.SendPasswordResetOtpAsync(user.Email, user.FullName, otp);
                }
                // TODO: else gửi SMS nếu identifier là SĐT
            }
            catch (Exception)
            {
                return AuthResultDto.Fail("Gửi OTP thất bại. Vui lòng thử lại sau.");
            }

            return AuthResultDto.Ok("OTP đã được gửi. Có hiệu lực trong 30 phút, chỉ dùng 1 lần.");
        }

        public async Task<AuthResultDto> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            // 1. Validate password mới
            var passwordErrors = _passwordValidator.Validate(request.NewPassword);
            if (passwordErrors.Count > 0 )
                return AuthResultDto.Fail("Mật khẩu không đủ mạnh: " + string.Join("; ", passwordErrors));

            if (request.NewPassword != request.ConfirmPassword)
                return AuthResultDto.Fail("Mật khẩu xác nhận không khớp.");

            // 2. Tìm OTP hợp lệ
            var otpRepo = _unitOfWork.GetRepository<OtpVerification>();
            var otp = await otpRepo.FirstOrDefaultAsync(o =>
                o.OtpCode == request.Token.Trim() &&
                o.Purpose == "ResetPassword" &&
                !o.IsUsed &&
                o.ExpiresAt > DateTime.UtcNow);

            if (otp == null)
                return AuthResultDto.Fail("OTP không hợp lệ hoặc đã hết hạn.");

            // 3. Đánh dấu OTP đã dùng
            otp.IsUsed = true;
            otp.UsedAt = DateTime.UtcNow;
            otpRepo.Update(otp);

            // 4. Đổi mật khẩu
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(otp.UserId!.Value);
            if (user == null)
                return AuthResultDto.Fail("Người dùng không tồn tại.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            userRepo.Update(user);

            // 5. Invalidate tất cả refresh tokens (đá hết sessions cũ)
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken>();
            var activeTokens = await refreshTokenRepo.FindAsync(rt =>
                rt.UserId == user.Id && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow);

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                refreshTokenRepo.Update(token);
            }

            await _unitOfWork.SaveChangesAsync();
            return AuthResultDto.Ok("Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại.");
        }
    }
}
