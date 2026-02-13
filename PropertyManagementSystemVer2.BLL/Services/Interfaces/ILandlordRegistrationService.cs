using PropertyManagementSystemVer2.BLL.DTOs.Auth;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface ILandlordRegistrationService
    {
        Task<AuthResultDto> SubmitAsync(int userId, SubmitLandlordRequestDto request);
        Task<AuthResultDto> ResubmitAsync(int userId, SubmitLandlordRequestDto request);
        Task<AuthResultDto> GetMyStatusAsync(int userId);
        Task<AuthResultDto> GetPendingListAsync(int page = 1, int pageSize = 20);
        Task<AuthResultDto> ReviewAsync(int adminId, ReviewLandlordRequestDto request);
    }
}
