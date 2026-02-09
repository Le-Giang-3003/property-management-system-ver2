using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.DTOs
{
    public class MaintenanceRequestDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public int LeaseId { get; set; }
        public int RequestedBy { get; set; }
        public string RequesterName { get; set; } = string.Empty;
        public MaintenanceStatus Status { get; set; }
        public MaintenancePriority Priority { get; set; }
        public MaintenanceCategory Category { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrls { get; set; }
        public int? AssignedTo { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? AssignedAt { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public string? Resolution { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public int? Rating { get; set; }
        public string? Feedback { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMaintenanceRequestDto
    {
        public int PropertyId { get; set; }
        public int LeaseId { get; set; }
        public MaintenancePriority Priority { get; set; }
        public MaintenanceCategory Category { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrls { get; set; }
    }

    public class UpdateMaintenanceRequestDto
    {
        public int RequestId { get; set; }
        public MaintenancePriority? Priority { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public decimal? EstimatedCost { get; set; }
    }

    public class AssignTechnicianDto
    {
        public int RequestId { get; set; }
        public int TechnicianId { get; set; }
    }

    public class CompleteMaintenanceDto
    {
        public int RequestId { get; set; }
        public string Resolution { get; set; } = string.Empty;
        public decimal? ActualCost { get; set; }
        public string? ImageUrls { get; set; }
    }

    public class RateMaintenanceDto
    {
        public int RequestId { get; set; }
        public int Rating { get; set; }
        public string? Feedback { get; set; }
    }

    public class MaintenanceSummaryDto
    {
        public int TotalRequests { get; set; }
        public int OpenCount { get; set; }
        public int InProgressCount { get; set; }
        public int ResolvedCount { get; set; }
        public double AverageResolutionDays { get; set; }
    }
}
