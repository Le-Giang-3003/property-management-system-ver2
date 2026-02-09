using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(DbContext context) : base(context) { }

        public async Task<Payment?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Lease).ThenInclude(l => l.Property)
                .Include(p => p.Lease).ThenInclude(l => l.Tenant)
                .Include(p => p.Lease).ThenInclude(l => l.Landlord)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Payment>> GetByLeaseIdAsync(int leaseId, PaymentStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(p => p.LeaseId == leaseId);
            if (status.HasValue) query = query.Where(p => p.Status == status.Value);
            return await query.OrderByDescending(p => p.DueDate).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Payment>> GetByTenantIdAsync(int tenantId, PaymentStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(p => p.Lease).ThenInclude(l => l.Property).Where(p => p.Lease.TenantId == tenantId);
            if (status.HasValue) query = query.Where(p => p.Status == status.Value);
            return await query.OrderByDescending(p => p.DueDate).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Payment>> GetByLandlordIdAsync(int landlordId, PaymentStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(p => p.Lease).ThenInclude(l => l.Property).Where(p => p.Lease.LandlordId == landlordId);
            if (status.HasValue) query = query.Where(p => p.Status == status.Value);
            return await query.OrderByDescending(p => p.DueDate).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Payment>> GetOverduePaymentsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Lease).ThenInclude(l => l.Tenant)
                .Where(p => p.Status == PaymentStatus.Pending && p.DueDate < DateTime.UtcNow)
                .OrderBy(p => p.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsDueSoonAsync(int daysBeforeDue, CancellationToken cancellationToken = default)
        {
            var dueDate = DateTime.UtcNow.AddDays(daysBeforeDue);
            return await _dbSet
                .Include(p => p.Lease).ThenInclude(l => l.Tenant)
                .Where(p => p.Status == PaymentStatus.Pending && p.DueDate <= dueDate && p.DueDate >= DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalPaidByLeaseAsync(int leaseId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(p => p.LeaseId == leaseId && p.Status == PaymentStatus.Completed).SumAsync(p => p.Amount, cancellationToken);
        }
    }
}
