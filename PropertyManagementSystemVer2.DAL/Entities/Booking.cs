using PropertyManagementSystemVer2.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public int TenantId { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Thông tin lịch hẹn
        public DateTime ScheduledDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string? Message { get; set; } // Lời nhắn từ Tenant

        // Thông tin xác nhận
        public DateTime? ConfirmedAt { get; set; }

        public string? ConfirmationNotes { get; set; } // Ghi chú từ Landlord khi xác nhận

        // Lý do hủy
        public string? CancellationReason { get; set; }

        public DateTime? CancelledAt { get; set; }

        public int? CancelledBy { get; set; } // User ID của người hủy

        // Thông tin hoàn thành
        public DateTime? CompletedAt { get; set; }

        public string? CompletionNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Property Property { get; set; } = null!;

        public User Tenant { get; set; } = null!;
    }
}
