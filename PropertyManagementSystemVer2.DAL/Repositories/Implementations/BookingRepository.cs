using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Data;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context) { }

        public async Task<Booking?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(b => b.Property).ThenInclude(p => p.Landlord)
                .Include(b => b.Tenant)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Booking>> GetByTenantIdAsync(int tenantId, BookingStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(b => b.Property).ThenInclude(p => p.Images.Where(i => i.IsPrimary)).Where(b => b.TenantId == tenantId);
            if (status.HasValue) query = query.Where(b => b.Status == status.Value);
            return await query.OrderByDescending(b => b.ScheduledDate).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Booking>> GetByPropertyIdAsync(int propertyId, BookingStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(b => b.Tenant).Where(b => b.PropertyId == propertyId);
            if (status.HasValue) query = query.Where(b => b.Status == status.Value);
            return await query.OrderByDescending(b => b.ScheduledDate).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Booking>> GetByLandlordIdAsync(int landlordId, BookingStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(b => b.Property).Include(b => b.Tenant).Where(b => b.Property.LandlordId == landlordId);
            if (status.HasValue) query = query.Where(b => b.Status == status.Value);
            return await query.OrderByDescending(b => b.ScheduledDate).ToListAsync(cancellationToken);
        }

        public async Task<int> CountPendingByTenantAndPropertyAsync(int tenantId, int propertyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(b => b.TenantId == tenantId && b.PropertyId == propertyId && b.Status == BookingStatus.Pending, cancellationToken);
        }

        public async Task<bool> IsSlotAvailableAsync(int propertyId, DateTime scheduledDate, TimeSpan startTime, TimeSpan endTime, CancellationToken cancellationToken = default)
        {
            return !await _dbSet.AnyAsync(b =>
                b.PropertyId == propertyId &&
                b.ScheduledDate == scheduledDate.Date &&
                b.Status != BookingStatus.Cancelled &&
                b.StartTime < endTime && b.EndTime > startTime, cancellationToken);
        }

        public async Task<IEnumerable<Booking>> GetExpiredPendingBookingsAsync(int hoursTimeout, CancellationToken cancellationToken = default)
        {
            var cutoff = DateTime.UtcNow.AddHours(-hoursTimeout);
            return await _dbSet.Where(b => b.Status == BookingStatus.Pending && b.CreatedAt < cutoff).ToListAsync(cancellationToken);
        }
    }
}
