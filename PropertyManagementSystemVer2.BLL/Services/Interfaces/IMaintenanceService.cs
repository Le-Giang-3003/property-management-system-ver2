using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<ServiceResultDto<MaintenanceRequestDto>> CreateRequestAsync(int tenantId, CreateMaintenanceRequestDto dto);
        Task<ServiceResultDto<MaintenanceRequestDto>> GetByIdAsync(int requestId);
        Task<ServiceResultDto<List<MaintenanceRequestDto>>> GetByPropertyIdAsync(int propertyId, MaintenanceStatus? status = null);
        Task<ServiceResultDto<List<MaintenanceRequestDto>>> GetByTenantIdAsync(int tenantId, MaintenanceStatus? status = null);
        Task<ServiceResultDto<List<MaintenanceRequestDto>>> GetByLandlordIdAsync(int landlordId, MaintenanceStatus? status = null);
        Task<ServiceResultDto> ReviewRequestAsync(int landlordId, UpdateMaintenanceRequestDto dto);
        Task<ServiceResultDto> LandlordApproveAsync(int landlordId, int requestId, string technicianName, string technicianPhone);
        Task<ServiceResultDto> LandlordRejectAsync(int landlordId, int requestId, string reason);
        Task<ServiceResultDto> LandlordCompleteAsync(int landlordId, int requestId, string resolution);
        Task<ServiceResultDto> ConfirmCompletionAsync(int userId, int requestId, bool isResolved);
        Task<ServiceResultDto<MaintenanceSummaryDto>> GetSummaryByPropertyAsync(int propertyId);
    }
}
