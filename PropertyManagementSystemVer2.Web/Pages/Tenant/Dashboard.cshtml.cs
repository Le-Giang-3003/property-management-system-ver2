using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Tenant
{
    [Authorize(Roles = "Tenant")]
    public class DashboardModel : PageModel
    {
        private readonly IDashboardService _dashboardService;

        public DashboardModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public TenantDashboardDto? DashboardData { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _dashboardService.GetTenantDashboardAsync(userId);
                if (result.IsSuccess)
                {
                    DashboardData = result.Data;
                }
                else
                {
                    DashboardData = new TenantDashboardDto();
                }
            }
            else
            {
                DashboardData = new TenantDashboardDto();
            }

            return Page();
        }
    }
}
