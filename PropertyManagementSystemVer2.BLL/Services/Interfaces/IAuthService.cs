using PropertyManagementSystemVer2.BLL.DTOs.Auth;

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

        Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<AuthResultDto> ResetPasswordAsync(ResetPasswordRequestDto request);

        Task<AuthResultDto> GetProfileAsync(int userId);
        Task<AuthResultDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto request);
        Task<AuthResultDto> UpdateLandlordInfoAsync(int userId, UpdateLandlordInfoRequestDto request);

        Task<AuthResultDto> SendPhoneOtpAsync(int userId, SendPhoneOtpRequestDto request);
        Task<AuthResultDto> VerifyPhoneOtpAsync(int userId, VerifyPhoneOtpRequestDto request);

    }
}
