using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly IDashboardService _dashboardService;

        public DashboardModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public AdminDashboardDto? DashboardData { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await _dashboardService.GetAdminDashboardAsync();
            if (result.IsSuccess)
            {
                DashboardData = result.Data;
            }
            else
            {
                // Fallback to empty if it fails
                DashboardData = new AdminDashboardDto();
            }

            return Page();
        }
    }
}
