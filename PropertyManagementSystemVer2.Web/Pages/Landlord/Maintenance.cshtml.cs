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
                var result = await _maintenanceService.GetByLandlordIdAsync(userId);
                if (result.IsSuccess && result.Data != null)
                {
                    MaintenanceRequests = result.Data;
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int requestId, string technicianName, string technicianPhone)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _maintenanceService.LandlordApproveAsync(userId, requestId, technicianName, technicianPhone);
                if (!result.IsSuccess)
                {
                    TempData["ErrorMessage"] = result.Message;
                }
                else
                {
                    TempData["SuccessMessage"] = result.Message;
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int requestId, string reason)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _maintenanceService.LandlordRejectAsync(userId, requestId, reason);
                if (!result.IsSuccess)
                {
                    TempData["ErrorMessage"] = result.Message;
                }
                else
                {
                    TempData["SuccessMessage"] = result.Message;
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int requestId, string resolution)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _maintenanceService.LandlordCompleteAsync(userId, requestId, resolution);
                if (!result.IsSuccess)
                {
                    TempData["ErrorMessage"] = result.Message;
                }
                else
                {
                    TempData["SuccessMessage"] = result.Message;
                }
            }
            return RedirectToPage();
        }
    }
}
