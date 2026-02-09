namespace PropertyManagementSystemVer2.BLL.DTOs
{
    public class LandlordDashboardDto
    {
        public int TotalProperties { get; set; }
        public decimal OccupancyRate { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal PendingPayments { get; set; }
        public int ActiveMaintenanceRequests { get; set; }
        public List<MonthlyRevenueDto> RevenueTrend { get; set; } = new();
    }

    public class TenantDashboardDto
    {
        public LeaseDto? ActiveLease { get; set; }
        public PaymentDto? NextPayment { get; set; }
        public int OpenMaintenanceRequests { get; set; }
        public List<BookingDto> UpcomingBookings { get; set; } = new();
    }

    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalProperties { get; set; }
        public int ActiveLeases { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingApprovals { get; set; }
        public List<MonthlyRevenueDto> RevenueTrend { get; set; } = new();
    }

    public class MonthlyRevenueDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class RevenueReportDto
    {
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public decimal TotalRentCollected { get; set; }
        public decimal TotalLateFees { get; set; }
        public decimal TotalMaintenanceCost { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal NetRevenue { get; set; }
    }

    public class OccupancyReportDto
    {
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public decimal OccupancyRate { get; set; }
        public int DaysOccupied { get; set; }
        public int TotalDays { get; set; }
    }
}
