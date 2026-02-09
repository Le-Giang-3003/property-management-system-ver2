using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR59: Landlord Dashboard
        // 1. Tổng quan: total properties, occupancy rate, monthly revenue, pending payments, active maintenance
        // 2. Charts: revenue trend (12 tháng), occupancy rate, payment collection rate
        public async Task<ServiceResultDto<LandlordDashboardDto>> GetLandlordDashboardAsync(int landlordId)
        {
            var properties = await _unitOfWork.Properties.GetByLandlordIdAsync(landlordId);
            var propertyList = properties.ToList();
            var propertyIds = propertyList.Select(p => p.Id).ToList();

            var totalProperties = propertyList.Count;
            var rentedCount = propertyList.Count(p => p.Status == PropertyStatus.Rented);
            var occupancyRate = totalProperties > 0 ? (decimal)rentedCount / totalProperties * 100 : 0;

            // Lấy payments từ leases của landlord
            var payments = await _unitOfWork.Payments.GetByLandlordIdAsync(landlordId);
            var paymentList = payments.ToList();
            var now = DateTime.UtcNow;
            var monthlyRevenue = paymentList
                .Where(p => p.Status == PaymentStatus.Completed && p.PaidDate?.Month == now.Month && p.PaidDate?.Year == now.Year)
                .Sum(p => p.Amount);
            var pendingPayments = paymentList.Where(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Overdue).Sum(p => p.Amount);

            // Active maintenance
            var activeMaintenanceCount = 0;
            foreach (var propertyId in propertyIds)
            {
                activeMaintenanceCount += await _unitOfWork.MaintenanceRequests.CountByStatusAndPropertyAsync(propertyId, MaintenanceStatus.Open);
                activeMaintenanceCount += await _unitOfWork.MaintenanceRequests.CountByStatusAndPropertyAsync(propertyId, MaintenanceStatus.InProgress);
            }

            // Revenue trend 12 tháng
            var revenueTrend = new List<MonthlyRevenueDto>();
            for (int i = 11; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var monthRevenue = paymentList
                    .Where(p => p.Status == PaymentStatus.Completed && p.PaidDate?.Month == month.Month && p.PaidDate?.Year == month.Year)
                    .Sum(p => p.Amount);
                revenueTrend.Add(new MonthlyRevenueDto { Year = month.Year, Month = month.Month, Revenue = monthRevenue });
            }

            var dashboard = new LandlordDashboardDto
            {
                TotalProperties = totalProperties,
                OccupancyRate = occupancyRate,
                MonthlyRevenue = monthlyRevenue,
                PendingPayments = pendingPayments,
                ActiveMaintenanceRequests = activeMaintenanceCount,
                RevenueTrend = revenueTrend
            };

            return ServiceResultDto<LandlordDashboardDto>.Success(dashboard);
        }

        // BR60: Tenant Dashboard
        // 1. Tổng quan: active lease info, next payment due, open maintenance requests, upcoming bookings
        public async Task<ServiceResultDto<TenantDashboardDto>> GetTenantDashboardAsync(int tenantId)
        {
            // Active lease
            var leases = await _unitOfWork.Leases.GetByTenantIdAsync(tenantId, LeaseStatus.Active);
            var activeLease = leases.FirstOrDefault();

            LeaseDto? leaseDto = null;
            PaymentDto? nextPaymentDto = null;

            if (activeLease != null)
            {
                leaseDto = new LeaseDto
                {
                    Id = activeLease.Id,
                    LeaseNumber = activeLease.LeaseNumber,
                    PropertyId = activeLease.PropertyId,
                    PropertyTitle = activeLease.Property?.Title ?? string.Empty,
                    PropertyAddress = activeLease.Property?.Address ?? string.Empty,
                    LandlordId = activeLease.LandlordId,
                    LandlordName = activeLease.Landlord?.FullName ?? string.Empty,
                    TenantId = activeLease.TenantId,
                    Status = activeLease.Status,
                    StartDate = activeLease.StartDate,
                    EndDate = activeLease.EndDate,
                    MonthlyRent = activeLease.MonthlyRent,
                    DepositAmount = activeLease.DepositAmount,
                    CreatedAt = activeLease.CreatedAt
                };

                // Next payment
                var payments = await _unitOfWork.Payments.GetByLeaseIdAsync(activeLease.Id, PaymentStatus.Pending);
                var nextPayment = payments.OrderBy(p => p.DueDate).FirstOrDefault();
                if (nextPayment != null)
                {
                    nextPaymentDto = new PaymentDto
                    {
                        Id = nextPayment.Id,
                        LeaseId = nextPayment.LeaseId,
                        PaymentType = nextPayment.PaymentType,
                        Status = nextPayment.Status,
                        Amount = nextPayment.Amount,
                        DueDate = nextPayment.DueDate,
                        Description = nextPayment.Description,
                        CreatedAt = nextPayment.CreatedAt
                    };
                }
            }

            // Open maintenance
            var maintenanceRequests = await _unitOfWork.MaintenanceRequests.GetByTenantIdAsync(tenantId, MaintenanceStatus.Open);
            var inProgressRequests = await _unitOfWork.MaintenanceRequests.GetByTenantIdAsync(tenantId, MaintenanceStatus.InProgress);
            var openCount = maintenanceRequests.Count() + inProgressRequests.Count();

            // Upcoming bookings
            var bookings = await _unitOfWork.Bookings.GetByTenantIdAsync(tenantId, BookingStatus.Confirmed);
            var upcomingBookings = bookings
                .Where(b => b.ScheduledDate >= DateTime.UtcNow.Date)
                .OrderBy(b => b.ScheduledDate)
                .Take(5)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    PropertyId = b.PropertyId,
                    PropertyTitle = b.Property?.Title ?? string.Empty,
                    Status = b.Status,
                    ScheduledDate = b.ScheduledDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    CreatedAt = b.CreatedAt
                }).ToList();

            var dashboard = new TenantDashboardDto
            {
                ActiveLease = leaseDto,
                NextPayment = nextPaymentDto,
                OpenMaintenanceRequests = openCount,
                UpcomingBookings = upcomingBookings
            };

            return ServiceResultDto<TenantDashboardDto>.Success(dashboard);
        }

        // BR61: Admin Dashboard
        // 1. System overview: total users, properties, active leases, revenue
        // 2. User growth trend, property listing trend, dispute statistics
        public async Task<ServiceResultDto<AdminDashboardDto>> GetAdminDashboardAsync()
        {
            var totalUsers = await _unitOfWork.Users.CountAsync();
            var totalProperties = await _unitOfWork.Properties.CountAsync();
            var activeLeases = await _unitOfWork.Leases.CountAsync(l => l.Status == LeaseStatus.Active);
            var pendingApprovals = await _unitOfWork.Properties.CountAsync(p => p.Status == PropertyStatus.Pending);

            // Total revenue
            var allPayments = await _unitOfWork.Payments.GetByLandlordIdAsync(0); // Simplified - get all
            // Workaround: get all completed payments
            var completedPayments = (await _unitOfWork.Payments.FindAsync(p => p.Status == PaymentStatus.Completed)).ToList();
            var totalRevenue = completedPayments.Sum(p => p.Amount);

            // Revenue trend
            var now = DateTime.UtcNow;
            var revenueTrend = new List<MonthlyRevenueDto>();
            for (int i = 11; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var monthRevenue = completedPayments
                    .Where(p => p.PaidDate?.Month == month.Month && p.PaidDate?.Year == month.Year)
                    .Sum(p => p.Amount);
                revenueTrend.Add(new MonthlyRevenueDto { Year = month.Year, Month = month.Month, Revenue = monthRevenue });
            }

            var dashboard = new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalProperties = totalProperties,
                ActiveLeases = activeLeases,
                TotalRevenue = totalRevenue,
                PendingApprovals = pendingApprovals,
                RevenueTrend = revenueTrend
            };

            return ServiceResultDto<AdminDashboardDto>.Success(dashboard);
        }

        // BR62: Revenue Report
        // 1. Revenue theo property, theo tháng/quý/năm
        // 2. Breakdown: rent collected, late fees, deductions
        public async Task<ServiceResultDto<List<RevenueReportDto>>> GetRevenueReportAsync(int? landlordId, int year, int? month = null)
        {
            var revenues = await _unitOfWork.Revenues.GetByYearAsync(year);

            if (landlordId.HasValue)
            {
                var landlordProperties = await _unitOfWork.Properties.GetByLandlordIdAsync(landlordId.Value);
                var propertyIds = landlordProperties.Select(p => p.Id).ToHashSet();
                revenues = revenues.Where(r => propertyIds.Contains(r.PropertyId));
            }

            if (month.HasValue)
                revenues = revenues.Where(r => r.Month == month.Value);

            var report = revenues.Select(r => new RevenueReportDto
            {
                PropertyId = r.PropertyId,
                PropertyTitle = r.Property?.Title ?? string.Empty,
                TotalRentCollected = r.TotalRentCollected,
                TotalLateFees = r.TotalLateFees,
                TotalMaintenanceCost = r.TotalMaintenanceCost,
                GrossRevenue = r.GrossRevenue,
                NetRevenue = r.NetRevenue
            }).ToList();

            return ServiceResultDto<List<RevenueReportDto>>.Success(report);
        }

        // BR63: Occupancy Report
        // 1. Tỷ lệ lấp đầy theo property, theo thời gian
        // 2. Average days vacant
        // 3. Comparison between properties
        public async Task<ServiceResultDto<List<OccupancyReportDto>>> GetOccupancyReportAsync(int? landlordId)
        {
            IEnumerable<DAL.Entities.Property> properties;
            if (landlordId.HasValue)
                properties = await _unitOfWork.Properties.GetByLandlordIdAsync(landlordId.Value);
            else
                properties = await _unitOfWork.Properties.GetAllAsync();

            var now = DateTime.UtcNow;
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var report = new List<OccupancyReportDto>();

            foreach (var property in properties)
            {
                var hasActiveLease = await _unitOfWork.Properties.HasActiveLeaseAsync(property.Id);
                report.Add(new OccupancyReportDto
                {
                    PropertyId = property.Id,
                    PropertyTitle = property.Title,
                    OccupancyRate = hasActiveLease ? 100 : 0,
                    DaysOccupied = hasActiveLease ? daysInMonth : 0,
                    TotalDays = daysInMonth
                });
            }

            return ServiceResultDto<List<OccupancyReportDto>>.Success(report);
        }
    }
}
