using PropertyManagementSystemVer2.BLL.DTOs;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ServiceResultDto<ReviewDto>> CreatePropertyReviewAsync(int tenantId, CreatePropertyReviewDto dto);
        Task<ServiceResultDto<ReviewDto>> CreateTenantReviewAsync(int landlordId, CreateTenantReviewDto dto);
        Task<ServiceResultDto<List<ReviewDto>>> GetPropertyReviewsAsync(int propertyId);
        Task<ServiceResultDto<List<ReviewDto>>> GetTenantReviewsAsync(int tenantId);
        Task<ServiceResultDto<ReviewSummaryDto>> GetPropertyReviewSummaryAsync(int propertyId);
        Task<ServiceResultDto> ReportReviewAsync(int userId, int reviewId, string reason);
    }
}
