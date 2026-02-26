using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class DashboardModel : PageModel
    {
        private readonly IDashboardService _dashboardService;

        public DashboardModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public LandlordDashboardDto? DashboardData { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _dashboardService.GetLandlordDashboardAsync(userId);
                if (result.IsSuccess)
                {
                    DashboardData = result.Data;
                }
                else
                {
                    DashboardData = new LandlordDashboardDto();
                }
            }
            else
            {
                DashboardData = new LandlordDashboardDto();
            }

            return Page();
        }
    }
}
