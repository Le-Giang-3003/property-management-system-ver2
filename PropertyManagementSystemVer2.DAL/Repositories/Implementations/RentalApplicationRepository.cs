using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class RentalApplicationRepository : GenericRepository<RentalApplication>, IRentalApplicationRepository
    {
        public RentalApplicationRepository(DbContext context) : base(context) { }

        public async Task<RentalApplication?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(a => a.Property).ThenInclude(p => p.Landlord)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<RentalApplication>> GetByPropertyIdAsync(int propertyId, ApplicationStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(a => a.Tenant).Where(a => a.PropertyId == propertyId);
            if (status.HasValue) query = query.Where(a => a.Status == status.Value);
            return await query.OrderByDescending(a => a.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RentalApplication>> GetByTenantIdAsync(int tenantId, ApplicationStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(a => a.Property).ThenInclude(p => p.Images.Where(i => i.IsPrimary)).Where(a => a.TenantId == tenantId);
            if (status.HasValue) query = query.Where(a => a.Status == status.Value);
            return await query.OrderByDescending(a => a.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<bool> HasActiveApplicationAsync(int tenantId, int propertyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(a => a.TenantId == tenantId && a.PropertyId == propertyId && a.Status == ApplicationStatus.Pending, cancellationToken);
        }
    }
}
