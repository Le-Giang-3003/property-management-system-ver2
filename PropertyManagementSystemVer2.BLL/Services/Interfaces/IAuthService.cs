using PropertyManagementSystemVer2.BLL.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginRequestDto request, string? ipAddress = null);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
        Task<AuthResultDto> RevokeRefreshTokenAsync(string refreshToken, string? ipAddress = null);
        Task<AuthResultDto> LogoutAsync(int userId, string refreshToken, string? ipAddress = null);

        Task<AuthResultDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResultDto> VerifyEmailAsync(string token);
        Task<AuthResultDto> ResendEmailVerificationAsync(string email);


    }
}
