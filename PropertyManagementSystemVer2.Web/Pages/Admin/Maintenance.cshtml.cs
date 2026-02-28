using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class MaintenanceModel : PageModel
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceModel(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public List<MaintenanceRequestDto> MaintenanceRequests { get; set; } = new List<MaintenanceRequestDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await _maintenanceService.GetAllRequestsAsync();
            if (result.IsSuccess && result.Data != null)
            {
                MaintenanceRequests = result.Data;
            }
            return Page();
        }
    }
}
