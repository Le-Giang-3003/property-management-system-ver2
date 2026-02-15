using PropertyManagementSystemVer2.BLL.DTOs.Admin;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AdminDashboardService(IUserRepository userRepo, IUnitOfWork unitOfWork)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var leaseRepo = _unitOfWork.GetRepository<Lease>();

            var dashboard = new AdminDashboardDto
            {
                TotalUsers = await _userRepo.CountAsync(),
                ActiveUsers = await _userRepo.CountAsync(u => u.IsActive)
            };

            //// Recent activities
            //var (recentLogs, _) = await _auditLogRepo.GetPagedAsync(
            //    null, null, null, null, null, 1, 10);

            //dashboard.RecentActivities = recentLogs.Select(a => new RecentActivityDto
            //{
            //    Description = a.Description ?? $"{a.ActionType} on {a.EntityName}",
            //    UserName = a.User?.FullName ?? "System",
            //    CreatedAt = a.CreatedAt,
            //    ActionType = a.ActionType.ToString()
            //}).ToList();

            return dashboard;
        }
    }
}
