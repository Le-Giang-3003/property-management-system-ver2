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
        Task<ServiceResultDto<List<MaintenanceRequestDto>>> GetByAssignedToAsync(int technicianId, MaintenanceStatus? status = null);
        Task<ServiceResultDto> ReviewRequestAsync(int landlordId, UpdateMaintenanceRequestDto dto);
        Task<ServiceResultDto> AssignTechnicianAsync(int landlordId, AssignTechnicianDto dto);
        Task<ServiceResultDto> LandlordApproveAsync(int landlordId, int requestId, string technicianName, string technicianPhone);
        Task<ServiceResultDto> LandlordRejectAsync(int landlordId, int requestId, string reason);
        Task<ServiceResultDto> LandlordCompleteAsync(int landlordId, int requestId, string resolution, decimal? actualCost);
        Task<ServiceResultDto> AcceptDeclineAssignmentAsync(int technicianId, int requestId, bool accept, string? reason = null);
        Task<ServiceResultDto> UpdateProgressAsync(int technicianId, int requestId, string notes, string? imageUrls = null);
        Task<ServiceResultDto> CompleteRequestAsync(int technicianId, CompleteMaintenanceDto dto);
        Task<ServiceResultDto> ConfirmCompletionAsync(int userId, int requestId, bool isResolved);
        Task<ServiceResultDto> RateMaintenanceAsync(int tenantId, RateMaintenanceDto dto);
        Task<ServiceResultDto<MaintenanceSummaryDto>> GetSummaryByPropertyAsync(int propertyId);
    }
}
