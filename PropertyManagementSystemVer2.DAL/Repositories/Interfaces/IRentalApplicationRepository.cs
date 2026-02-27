using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IRentalApplicationRepository : IGenericRepository<RentalApplication>
    {
        Task<RentalApplication?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<RentalApplication>> GetByPropertyIdAsync(int propertyId, ApplicationStatus? status = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<RentalApplication>> GetByTenantIdAsync(int tenantId, ApplicationStatus? status = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<RentalApplication>> GetByLandlordIdAsync(int landlordId, ApplicationStatus? status = null, CancellationToken cancellationToken = default);
        Task<bool> HasActiveApplicationAsync(int tenantId, int propertyId, CancellationToken cancellationToken = default);
    }
}
