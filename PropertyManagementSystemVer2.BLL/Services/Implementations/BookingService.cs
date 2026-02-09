using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR15: Tạo Booking xem nhà
        // 1. Chọn ngày giờ xem nhà (phải trong tương lai, ≥24h trước)
        // 2. Mỗi slot 30 phút, không trùng slot đã book
        // 3. Gửi notification cho Landlord
        // 4. Max 3 pending bookings/tenant/property
        public async Task<ServiceResultDto<BookingDto>> CreateBookingAsync(int tenantId, CreateBookingDto dto)
        {
            var property = await _unitOfWork.Properties.GetByIdWithDetailsAsync(dto.PropertyId);
            if (property == null)
                return ServiceResultDto<BookingDto>.Failure("Không tìm thấy property.");

            if (property.Status != PropertyStatus.Approved)
                return ServiceResultDto<BookingDto>.Failure("Property không khả dụng để đặt lịch xem.");

            // BR15.1: Ngày giờ phải trong tương lai, ít nhất 24h trước
            var scheduledDateTime = dto.ScheduledDate.Date + dto.StartTime;
            if (scheduledDateTime <= DateTime.UtcNow.AddHours(24))
                return ServiceResultDto<BookingDto>.Failure("Phải đặt lịch ít nhất 24 giờ trước thời gian xem.");

            // BR15.2: Mỗi slot 30 phút
            var endTime = dto.StartTime.Add(TimeSpan.FromMinutes(30));

            // Kiểm tra trùng slot
            var isAvailable = await _unitOfWork.Bookings.IsSlotAvailableAsync(dto.PropertyId, dto.ScheduledDate, dto.StartTime, endTime);
            if (!isAvailable)
                return ServiceResultDto<BookingDto>.Failure("Khung giờ này đã có người đặt.");

            // BR15.4: Max 3 pending bookings/tenant/property
            var pendingCount = await _unitOfWork.Bookings.CountPendingByTenantAndPropertyAsync(tenantId, dto.PropertyId);
            if (pendingCount >= 3)
                return ServiceResultDto<BookingDto>.Failure("Bạn đã có tối đa 3 lịch hẹn pending cho property này.");

            var booking = new Booking
            {
                PropertyId = dto.PropertyId,
                TenantId = tenantId,
                Status = BookingStatus.Pending,
                ScheduledDate = dto.ScheduledDate.Date,
                StartTime = dto.StartTime,
                EndTime = endTime,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // BR15.3: TODO - Gửi notification cho Landlord
            var result = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(booking.Id);
            return ServiceResultDto<BookingDto>.Success(MapToBookingDto(result!));
        }

        // BR16: Landlord xác nhận/từ chối Booking
        // 1. Landlord confirm hoặc reject trong 48h
        // 2. Quá hạn auto-cancel (xử lý ở AutoCancelExpiredBookingsAsync)
        // 3. Khi confirm → notify Tenant
        // 4. Khi reject → phải nêu lý do, notify Tenant
        public async Task<ServiceResultDto> ConfirmRejectBookingAsync(int landlordId, ConfirmRejectBookingDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(dto.BookingId);
            if (booking == null)
                return ServiceResultDto.Failure("Không tìm thấy lịch hẹn.");

            if (booking.Property.LandlordId != landlordId)
                return ServiceResultDto.Failure("Bạn không có quyền thao tác với lịch hẹn này.");

            if (booking.Status != BookingStatus.Pending)
                return ServiceResultDto.Failure("Lịch hẹn không ở trạng thái chờ xác nhận.");

            if (dto.IsConfirmed)
            {
                // BR16.3: Confirm → notify Tenant
                booking.Status = BookingStatus.Confirmed;
                booking.ConfirmedAt = DateTime.UtcNow;
                booking.ConfirmationNotes = dto.Notes;
            }
            else
            {
                // BR16.4: Reject phải nêu lý do
                if (string.IsNullOrWhiteSpace(dto.Notes))
                    return ServiceResultDto.Failure("Phải nêu lý do từ chối.");

                booking.Status = BookingStatus.Cancelled;
                booking.CancellationReason = dto.Notes;
                booking.CancelledAt = DateTime.UtcNow;
                booking.CancelledBy = landlordId;
            }

            booking.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success(dto.IsConfirmed ? "Đã xác nhận lịch hẹn." : "Đã từ chối lịch hẹn.");
        }

        // BR17: Hủy Booking
        // 1. Hủy trước giờ hẹn ≥12h
        // 2. Hủy muộn bị đánh dấu 'late cancel' (ảnh hưởng rating)
        // 3. Auto-cancel nếu property bị unpublish (xử lý ở PropertyService.UnpublishAsync)
        public async Task<ServiceResultDto> CancelBookingAsync(int userId, CancelBookingDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(dto.BookingId);
            if (booking == null)
                return ServiceResultDto.Failure("Không tìm thấy lịch hẹn.");

            // Kiểm tra quyền: Tenant sở hữu hoặc Landlord sở hữu property
            if (booking.TenantId != userId && booking.Property.LandlordId != userId)
                return ServiceResultDto.Failure("Bạn không có quyền hủy lịch hẹn này.");

            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
                return ServiceResultDto.Failure("Không thể hủy lịch hẹn ở trạng thái hiện tại.");

            // BR17.1: Kiểm tra hủy trước 12h
            var scheduledDateTime = booking.ScheduledDate.Date + booking.StartTime;
            var isLateCancel = scheduledDateTime <= DateTime.UtcNow.AddHours(12);

            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = dto.CancellationReason;
            booking.CancelledAt = DateTime.UtcNow;
            booking.CancelledBy = userId;

            // BR17.2: Đánh dấu late cancel trong notes
            if (isLateCancel)
            {
                booking.CompletionNotes = "LATE_CANCEL";
            }

            booking.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success(isLateCancel ? "Đã hủy lịch hẹn (hủy muộn)." : "Đã hủy lịch hẹn.");
        }

        // BR16.2: Auto-cancel booking quá hạn 48h
        public async Task<ServiceResultDto> AutoCancelExpiredBookingsAsync()
        {
            var expiredBookings = await _unitOfWork.Bookings.GetExpiredPendingBookingsAsync(48);
            foreach (var booking in expiredBookings)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.CancellationReason = "Tự động hủy do quá hạn xác nhận 48 giờ.";
                booking.CancelledAt = DateTime.UtcNow;
                booking.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Bookings.Update(booking);
            }

            await _unitOfWork.SaveChangesAsync();
            return ServiceResultDto.Success($"Đã tự động hủy {expiredBookings.Count()} lịch hẹn quá hạn.");
        }

        public async Task<ServiceResultDto<BookingDto>> GetByIdAsync(int bookingId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(bookingId);
            if (booking == null)
                return ServiceResultDto<BookingDto>.Failure("Không tìm thấy lịch hẹn.");

            return ServiceResultDto<BookingDto>.Success(MapToBookingDto(booking));
        }

        // BR18: Lịch sử Booking
        // 1. Xem danh sách booking theo status (Pending/Confirmed/Completed/Cancelled)
        // 3. Filter theo ngày (xử lý thêm ở tầng Web nếu cần)
        public async Task<ServiceResultDto<List<BookingDto>>> GetByTenantIdAsync(int tenantId, BookingStatus? status = null)
        {
            var bookings = await _unitOfWork.Bookings.GetByTenantIdAsync(tenantId, status);
            return ServiceResultDto<List<BookingDto>>.Success(bookings.Select(MapToBookingDto).ToList());
        }

        // BR18.2: Landlord xem theo property
        public async Task<ServiceResultDto<List<BookingDto>>> GetByLandlordIdAsync(int landlordId, BookingStatus? status = null)
        {
            var bookings = await _unitOfWork.Bookings.GetByLandlordIdAsync(landlordId, status);
            return ServiceResultDto<List<BookingDto>>.Success(bookings.Select(MapToBookingDto).ToList());
        }

        public async Task<ServiceResultDto<List<BookingDto>>> GetByPropertyIdAsync(int propertyId, BookingStatus? status = null)
        {
            var bookings = await _unitOfWork.Bookings.GetByPropertyIdAsync(propertyId, status);
            return ServiceResultDto<List<BookingDto>>.Success(bookings.Select(MapToBookingDto).ToList());
        }

        private static BookingDto MapToBookingDto(Booking b)
        {
            return new BookingDto
            {
                Id = b.Id,
                PropertyId = b.PropertyId,
                PropertyTitle = b.Property?.Title ?? string.Empty,
                PropertyThumbnail = b.Property?.Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                TenantId = b.TenantId,
                TenantName = b.Tenant?.FullName ?? string.Empty,
                LandlordName = b.Property?.Landlord?.FullName ?? string.Empty,
                Status = b.Status,
                ScheduledDate = b.ScheduledDate,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Message = b.Message,
                ConfirmationNotes = b.ConfirmationNotes,
                CancellationReason = b.CancellationReason,
                CreatedAt = b.CreatedAt
            };
        }
    }
}
