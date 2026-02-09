using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string? PropertyThumbnail { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string LandlordName { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Message { get; set; }
        public string? ConfirmationNotes { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBookingDto
    {
        public int PropertyId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public string? Message { get; set; }
    }

    public class ConfirmRejectBookingDto
    {
        public int BookingId { get; set; }
        public bool IsConfirmed { get; set; }
        public string? Notes { get; set; }
    }

    public class CancelBookingDto
    {
        public int BookingId { get; set; }
        public string? CancellationReason { get; set; }
    }
}
