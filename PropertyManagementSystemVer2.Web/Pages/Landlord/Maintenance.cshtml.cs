using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Landlord
{
    [Authorize(Roles = "Landlord")]
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
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                // In a real scenario, we might need a GetByLandlordIdAsync method in IMaintenanceService,
                // or we fetch Landlord's properties, then fetch maintenance for each property.
                // For demonstration, we leave the list empty or you can implement the property iteration later.
                MaintenanceRequests = new List<MaintenanceRequestDto>();
            }

            return Page();
        }
    }
}
