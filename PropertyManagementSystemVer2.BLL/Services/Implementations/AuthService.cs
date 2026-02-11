using PropertyManagementSystemVer2.BLL.DTOs.Auth;
using PropertyManagementSystemVer2.BLL.Mapping;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;
        private const int OtpExpiryMinutes = 30;
        private const int EmailVerifyExpiryHours = 24;

        public AuthService(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
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
    }
}
