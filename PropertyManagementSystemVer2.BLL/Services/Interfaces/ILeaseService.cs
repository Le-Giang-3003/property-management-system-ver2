using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface ILeaseService
    {
        Task<ServiceResultDto<LeaseDto>> CreateLeaseAsync(int landlordId, CreateLeaseDto dto);
        Task<ServiceResultDto<LeaseDto>> GetByIdAsync(int leaseId);
        Task<ServiceResultDto<List<LeaseDto>>> GetByTenantIdAsync(int tenantId, LeaseStatus? status = null);
        Task<ServiceResultDto<List<LeaseDto>>> GetByLandlordIdAsync(int landlordId, LeaseStatus? status = null);
        Task<ServiceResultDto> SignLeaseAsync(int userId, int leaseId);
        Task<ServiceResultDto> RenewLeaseAsync(int landlordId, RenewLeaseDto dto);
        Task<ServiceResultDto> AcceptRejectRenewalAsync(int tenantId, int leaseId, bool accept);
        Task<ServiceResultDto> RequestEarlyTerminationAsync(int userId, EarlyTerminationDto dto);
        Task<ServiceResultDto> ConfirmTerminationAsync(int userId, int leaseId);
        Task<ServiceResultDto<List<LeaseDto>>> GetExpiringLeasesAsync(int daysBeforeExpiry = 30);
    }
}
