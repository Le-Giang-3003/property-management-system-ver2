using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Tenant
{
    [Authorize(Roles = "Tenant")]
    public class ApplicationsModel : PageModel
    {
        private readonly IRentalApplicationService _rentalApplicationService;

        public ApplicationsModel(IRentalApplicationService rentalApplicationService)
        {
            _rentalApplicationService = rentalApplicationService;
        }

        public List<RentalApplicationDto> Applications { get; set; } = new List<RentalApplicationDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _rentalApplicationService.GetByTenantIdAsync(userId);
                
                if (result.IsSuccess && result.Data != null)
                {
                    Applications = result.Data;
                }
            }

            return Page();
        }
    }
}
