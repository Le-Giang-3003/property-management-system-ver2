using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IRentalApplicationService
    {
        Task<ServiceResultDto<RentalApplicationDto>> SubmitApplicationAsync(int tenantId, CreateRentalApplicationDto dto);
        Task<ServiceResultDto<RentalApplicationDto>> GetByIdAsync(int applicationId);
        Task<ServiceResultDto<List<RentalApplicationDto>>> GetByPropertyIdAsync(int propertyId, ApplicationStatus? status = null);
        Task<ServiceResultDto<List<RentalApplicationDto>>> GetByTenantIdAsync(int tenantId, ApplicationStatus? status = null);
        Task<ServiceResultDto<List<RentalApplicationDto>>> GetByLandlordIdAsync(int landlordId, ApplicationStatus? status = null);
        Task<ServiceResultDto> ApproveRejectApplicationAsync(int landlordId, ApproveRejectApplicationDto dto);
        Task<ServiceResultDto> WithdrawApplicationAsync(int tenantId, int applicationId);
    }
}
