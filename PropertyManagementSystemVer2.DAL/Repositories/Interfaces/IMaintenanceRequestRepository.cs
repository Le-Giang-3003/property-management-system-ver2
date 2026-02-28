using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IMaintenanceRequestRepository : IGenericRepository<MaintenanceRequest>
    {
        Task<MaintenanceRequest?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<MaintenanceRequest>> GetByPropertyIdAsync(int propertyId, MaintenanceStatus? status = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<MaintenanceRequest>> GetByTenantIdAsync(int tenantId, MaintenanceStatus? status = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<MaintenanceRequest>> GetByLeaseIdAsync(int leaseId, CancellationToken cancellationToken = default);
        Task<int> CountByStatusAndPropertyAsync(int propertyId, MaintenanceStatus status, CancellationToken cancellationToken = default);
        Task<IEnumerable<MaintenanceRequest>> GetByLandlordIdAsync(int landlordId, MaintenanceStatus? status = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<MaintenanceRequest>> GetAllWithDetailsAsync(MaintenanceStatus? status = null, CancellationToken cancellationToken = default);
    }
}
