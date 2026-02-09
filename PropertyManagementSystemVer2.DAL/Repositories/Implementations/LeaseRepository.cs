using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class LeaseRepository : GenericRepository<Lease>, ILeaseRepository
    {
        public LeaseRepository(DbContext context) : base(context) { }

        public async Task<Lease?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(l => l.Property).ThenInclude(p => p.Images.Where(i => i.IsPrimary))
                .Include(l => l.Landlord)
                .Include(l => l.Tenant)
                .Include(l => l.Payments.OrderByDescending(p => p.DueDate))
                .Include(l => l.MaintenanceRequests)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Lease>> GetByTenantIdAsync(int tenantId, LeaseStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(l => l.Property).ThenInclude(p => p.Images.Where(i => i.IsPrimary)).Include(l => l.Landlord).Where(l => l.TenantId == tenantId);
            if (status.HasValue) query = query.Where(l => l.Status == status.Value);
            return await query.OrderByDescending(l => l.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Lease>> GetByLandlordIdAsync(int landlordId, LeaseStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(l => l.Property).Include(l => l.Tenant).Where(l => l.LandlordId == landlordId);
            if (status.HasValue) query = query.Where(l => l.Status == status.Value);
            return await query.OrderByDescending(l => l.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Lease>> GetByPropertyIdAsync(int propertyId, LeaseStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(l => l.Tenant).Include(l => l.Landlord).Where(l => l.PropertyId == propertyId);
            if (status.HasValue) query = query.Where(l => l.Status == status.Value);
            return await query.OrderByDescending(l => l.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<Lease?> GetActiveLeaseByPropertyAsync(int propertyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(l => l.Tenant)
                .Include(l => l.Landlord)
                .FirstOrDefaultAsync(l => l.PropertyId == propertyId && l.Status == LeaseStatus.Active, cancellationToken);
        }

        public async Task<IEnumerable<Lease>> GetExpiringLeasesAsync(int daysBeforeExpiry, CancellationToken cancellationToken = default)
        {
            var expiryDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);
            return await _dbSet
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Include(l => l.Landlord)
                .Where(l => l.Status == LeaseStatus.Active && l.EndDate <= expiryDate && l.EndDate > DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task<string> GenerateLeaseNumberAsync(CancellationToken cancellationToken = default)
        {
            var count = await _dbSet.CountAsync(cancellationToken);
            return $"LS-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D4}";
        }
    }
}
