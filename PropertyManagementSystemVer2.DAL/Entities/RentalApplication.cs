using PropertyManagementSystemVer2.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class RentalApplication
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public int TenantId { get; set; }

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        // Thông tin đơn xin thuê
        public DateTime MoveInDate { get; set; } // Ngày dự kiến chuyển vào

        public int LeaseDurationMonths { get; set; } // Thời hạn thuê (tháng)

        public int NumberOfOccupants { get; set; } // Số người ở

        public string? Message { get; set; } // Lời nhắn từ Tenant

        // Thông tin nghề nghiệp & tài chính
        public string? Occupation { get; set; }

        public decimal? MonthlyIncome { get; set; }

        public string? EmployerName { get; set; }

        public string? EmployerContact { get; set; }

        // Người tham chiếu
        public string? ReferenceName { get; set; }

        public string? ReferenceContact { get; set; }

        public string? ReferenceRelationship { get; set; }

        // Lý do từ chối (nếu Landlord từ chối)
        public string? RejectionReason { get; set; }

        public DateTime? ReviewedAt { get; set; } // Thời điểm Landlord xét duyệt

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Property Property { get; set; } = null!;

        public User Tenant { get; set; } = null!;
    }
}
