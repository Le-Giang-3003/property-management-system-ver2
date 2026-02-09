using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ServiceResultDto<BookingDto>> CreateBookingAsync(int tenantId, CreateBookingDto dto);
        Task<ServiceResultDto<BookingDto>> GetByIdAsync(int bookingId);
        Task<ServiceResultDto> ConfirmRejectBookingAsync(int landlordId, ConfirmRejectBookingDto dto);
        Task<ServiceResultDto> CancelBookingAsync(int userId, CancelBookingDto dto);
        Task<ServiceResultDto<List<BookingDto>>> GetByTenantIdAsync(int tenantId, BookingStatus? status = null);
        Task<ServiceResultDto<List<BookingDto>>> GetByLandlordIdAsync(int landlordId, BookingStatus? status = null);
        Task<ServiceResultDto<List<BookingDto>>> GetByPropertyIdAsync(int propertyId, BookingStatus? status = null);
        Task<ServiceResultDto> AutoCancelExpiredBookingsAsync();
    }
}
