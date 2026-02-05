using PropertyManagementSystemVer2.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class Lease
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public int LandlordId { get; set; }

        public int TenantId { get; set; }

        public int? RentalApplicationId { get; set; } // Link đến đơn xin thuê gốc

        public LeaseStatus Status { get; set; } = LeaseStatus.Pending;

        // Thông tin hợp đồng
        public string LeaseNumber { get; set; } = string.Empty; // Mã hợp đồng

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal MonthlyRent { get; set; }

        public decimal DepositAmount { get; set; }

        public string Currency { get; set; } = "VND";

        // Điều khoản thanh toán
        public int PaymentDueDay { get; set; } = 1; // Ngày đến hạn thanh toán mỗi tháng

        public decimal? LateFeePercentage { get; set; } // Phí trả chậm (%)

        // Điều khoản hợp đồng
        public string? Terms { get; set; } // Điều khoản chi tiết

        public string? SpecialConditions { get; set; } // Điều khoản đặc biệt

        // Chữ ký điện tử
        public bool LandlordSigned { get; set; }

        public DateTime? LandlordSignedAt { get; set; }

        public bool TenantSigned { get; set; }

        public DateTime? TenantSignedAt { get; set; }

        // Lý do chấm dứt hợp đồng
        public string? TerminationReason { get; set; }

        public DateTime? TerminatedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Property Property { get; set; } = null!;

        public User Landlord { get; set; } = null!;

        public User Tenant { get; set; } = null!;

        public RentalApplication? RentalApplication { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    }
}
