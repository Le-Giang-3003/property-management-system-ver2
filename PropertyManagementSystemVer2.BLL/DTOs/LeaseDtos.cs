using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.DTOs
{
    public class LeaseDto
    {
        public int Id { get; set; }
        public string LeaseNumber { get; set; } = string.Empty;
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public int LandlordId { get; set; }
        public string LandlordName { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public LeaseStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public string Currency { get; set; } = "VND";
        public int PaymentDueDay { get; set; }
        public decimal? LateFeePercentage { get; set; }
        public string? Terms { get; set; }
        public string? SpecialConditions { get; set; }
        public bool LandlordSigned { get; set; }
        public DateTime? LandlordSignedAt { get; set; }
        public bool TenantSigned { get; set; }
        public DateTime? TenantSignedAt { get; set; }
        public string? TerminationReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateLeaseDto
    {
        public int RentalApplicationId { get; set; }
        public int PropertyId { get; set; }
        public int TenantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public int PaymentDueDay { get; set; } = 5;
        public decimal? LateFeePercentage { get; set; }
        public string? Terms { get; set; }
        public string? SpecialConditions { get; set; }
    }

    public class RenewLeaseDto
    {
        public int LeaseId { get; set; }
        public DateTime NewEndDate { get; set; }
        public decimal? NewMonthlyRent { get; set; }
    }

    public class EarlyTerminationDto
    {
        public int LeaseId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
