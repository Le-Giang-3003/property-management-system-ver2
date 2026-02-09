using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class MaintenanceRequestRepository : GenericRepository<MaintenanceRequest>, IMaintenanceRequestRepository
    {
        public MaintenanceRequestRepository(DbContext context) : base(context) { }

        public async Task<MaintenanceRequest?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(m => m.Property).ThenInclude(p => p.Landlord)
                .Include(m => m.Lease)
                .Include(m => m.Requester)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetByPropertyIdAsync(int propertyId, MaintenanceStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(m => m.Requester).Where(m => m.PropertyId == propertyId);
            if (status.HasValue) query = query.Where(m => m.Status == status.Value);
            return await query.OrderByDescending(m => m.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetByTenantIdAsync(int tenantId, MaintenanceStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(m => m.Property).Where(m => m.RequestedBy == tenantId);
            if (status.HasValue) query = query.Where(m => m.Status == status.Value);
            return await query.OrderByDescending(m => m.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetByAssignedToAsync(int technicianId, MaintenanceStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(m => m.Property).Include(m => m.Requester).Where(m => m.AssignedTo == technicianId);
            if (status.HasValue) query = query.Where(m => m.Status == status.Value);
            return await query.OrderByDescending(m => m.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetByLeaseIdAsync(int leaseId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Include(m => m.Property).Where(m => m.LeaseId == leaseId).OrderByDescending(m => m.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<int> CountByStatusAndPropertyAsync(int propertyId, MaintenanceStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(m => m.PropertyId == propertyId && m.Status == status, cancellationToken);
        }
    }
}
