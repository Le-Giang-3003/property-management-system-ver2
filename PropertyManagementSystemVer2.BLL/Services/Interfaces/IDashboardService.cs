using PropertyManagementSystemVer2.BLL.DTOs;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ServiceResultDto<LandlordDashboardDto>> GetLandlordDashboardAsync(int landlordId);
        Task<ServiceResultDto<TenantDashboardDto>> GetTenantDashboardAsync(int tenantId);
        Task<ServiceResultDto<AdminDashboardDto>> GetAdminDashboardAsync();
        Task<ServiceResultDto<List<RevenueReportDto>>> GetRevenueReportAsync(int? landlordId, int year, int? month = null);
        Task<ServiceResultDto<List<OccupancyReportDto>>> GetOccupancyReportAsync(int? landlordId);
    }
}
