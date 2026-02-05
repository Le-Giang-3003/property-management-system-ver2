using PropertyManagementSystemVer2.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int LeaseId { get; set; }

        public PaymentType PaymentType { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public PaymentMethod? PaymentMethod { get; set; }

        // Thông tin thanh toán
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "VND";

        public DateTime DueDate { get; set; } // Ngày đến hạn

        public DateTime? PaidDate { get; set; } // Ngày thực tế thanh toán

        // Thông tin kỳ thanh toán (cho tiền thuê hàng tháng)
        public int? BillingMonth { get; set; } // Tháng thanh toán (1-12)

        public int? BillingYear { get; set; } // Năm thanh toán

        // Phí trả chậm
        public decimal? LateFeeAmount { get; set; }

        public string? Description { get; set; } // Mô tả chi tiết

        // Thông tin giao dịch
        public string? TransactionId { get; set; } // Mã giao dịch (từ payment gateway)

        public string? PaymentProof { get; set; } // URL hình ảnh chứng từ thanh toán

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Lease Lease { get; set; } = null!;
    }
}
