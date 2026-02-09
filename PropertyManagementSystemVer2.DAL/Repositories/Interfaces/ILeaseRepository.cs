using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface ILeaseRepository : IGenericRepository<Lease>
    {
        Task<Lease?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Lease>> GetByTenantIdAsync(int tenantId, LeaseStatus? status = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Lease>> GetByLandlordIdAsync(int landlordId, LeaseStatus? status = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Lease>> GetByPropertyIdAsync(int propertyId, LeaseStatus? status = null, CancellationToken cancellationToken = default);
        Task<Lease?> GetActiveLeaseByPropertyAsync(int propertyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Lease>> GetExpiringLeasesAsync(int daysBeforeExpiry, CancellationToken cancellationToken = default);
        Task<string> GenerateLeaseNumberAsync(CancellationToken cancellationToken = default);
    }
}
