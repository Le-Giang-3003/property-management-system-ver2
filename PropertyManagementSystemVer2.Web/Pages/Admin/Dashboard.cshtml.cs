using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs.Admin;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.Web.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IAdminDashboardService _dashboardService;
        public AdminDashboardDto Dashboard { get; set; } = new();

        public DashboardModel(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task OnGetAsync()
        {
            Dashboard = await _dashboardService.GetDashboardAsync();
            ViewData["AdminName"] = HttpContext.Session.GetString("FullName") ?? "Admin";
            //ViewData["PendingReports"] = Dashboard.PendingReports;
            //ViewData["OpenDisputes"] = Dashboard.OpenDisputes;
        }
    }
}
