using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int LeaseId { get; set; }
        public string LeaseNumber { get; set; } = string.Empty;
        public string PropertyTitle { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string LandlordName { get; set; } = string.Empty;
        public PaymentType PaymentType { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public int? BillingMonth { get; set; }
        public int? BillingYear { get; set; }
        public decimal? LateFeeAmount { get; set; }
        public string? Description { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentProof { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin ngân hàng Landlord (dùng để tạo VietQR)
        public string? LandlordBankAccount { get; set; }
        public string? LandlordBankName { get; set; }
        public string? LandlordBankAccountHolder { get; set; }
    }

    public class MakePaymentDto
    {
        public int PaymentId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentProof { get; set; }
        public string? Notes { get; set; }
    }

    public class ConfirmPaymentDto
    {
        public int PaymentId { get; set; }
        public bool IsConfirmed { get; set; }
        public string? Notes { get; set; }
    }

    public class RefundDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class PaymentSummaryDto
    {
        public decimal TotalPaid { get; set; }
        public decimal TotalPending { get; set; }
        public decimal TotalOverdue { get; set; }
        public int PaidCount { get; set; }
        public int PendingCount { get; set; }
        public int OverdueCount { get; set; }
    }
}
