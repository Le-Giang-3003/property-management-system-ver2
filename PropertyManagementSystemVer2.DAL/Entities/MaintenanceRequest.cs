using PropertyManagementSystemVer2.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class MaintenanceRequest
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public int LeaseId { get; set; }

        public int RequestedBy { get; set; } // Tenant ID

        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Open;

        public MaintenancePriority Priority { get; set; } = MaintenancePriority.Medium;

        public MaintenanceCategory Category { get; set; }

        // Thông tin yêu cầu
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? ImageUrls { get; set; } // JSON array của URLs hình ảnh

        // Thông tin xử lý
        public int? AssignedTo { get; set; } // ID của người xử lý (có thể là Landlord hoặc thợ)

        public DateTime? AssignedAt { get; set; }

        public decimal? EstimatedCost { get; set; }

        public decimal? ActualCost { get; set; }

        public string? Resolution { get; set; } // Mô tả cách giải quyết

        public DateTime? ResolvedAt { get; set; }

        public DateTime? ScheduledDate { get; set; } // Ngày hẹn xử lý

        // Đánh giá
        public int? Rating { get; set; } // 1-5 sao

        public string? Feedback { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Property Property { get; set; } = null!;

        public Lease Lease { get; set; } = null!;

        public User Requester { get; set; } = null!;
    }
}
