using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<Booking?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Booking>> GetByTenantIdAsync(int tenantId, BookingStatus? status = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Booking>> GetByPropertyIdAsync(int propertyId, BookingStatus? status = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Booking>> GetByLandlordIdAsync(int landlordId, BookingStatus? status = null, CancellationToken cancellationToken = default);
        Task<int> CountPendingByTenantAndPropertyAsync(int tenantId, int propertyId, CancellationToken cancellationToken = default);
        Task<bool> IsSlotAvailableAsync(int propertyId, DateTime scheduledDate, TimeSpan startTime, TimeSpan endTime, CancellationToken cancellationToken = default);
        Task<IEnumerable<Booking>> GetExpiredPendingBookingsAsync(int hoursTimeout, CancellationToken cancellationToken = default);
    }
}
