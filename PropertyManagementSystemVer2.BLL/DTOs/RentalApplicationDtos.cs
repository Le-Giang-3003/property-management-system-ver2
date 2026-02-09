using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.DTOs
{
    public class RentalApplicationDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string? PropertyThumbnail { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string TenantEmail { get; set; } = string.Empty;
        public string TenantPhone { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public DateTime MoveInDate { get; set; }
        public int LeaseDurationMonths { get; set; }
        public int NumberOfOccupants { get; set; }
        public string? Message { get; set; }
        public string? Occupation { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string? EmployerName { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRentalApplicationDto
    {
        public int PropertyId { get; set; }
        public DateTime MoveInDate { get; set; }
        public int LeaseDurationMonths { get; set; }
        public int NumberOfOccupants { get; set; }
        public string? Message { get; set; }
        public string? Occupation { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string? EmployerName { get; set; }
        public string? EmployerContact { get; set; }
        public string? ReferenceName { get; set; }
        public string? ReferenceContact { get; set; }
        public string? ReferenceRelationship { get; set; }
    }

    public class ApproveRejectApplicationDto
    {
        public int ApplicationId { get; set; }
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }
}
