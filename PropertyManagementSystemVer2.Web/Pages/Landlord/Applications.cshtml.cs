using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Landlord
{
    [Authorize(Roles = "Landlord")]
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
                // This would realistically need a GetByLandlordIdAsync method. 
                // Since IRentalApplicationService doesn't have it, we might fetch properties first, then loop over them, 
                // or assume we have a custom method. For demo:
                Applications = new List<RentalApplicationDto>();
            }

            return Page();
        }
    }
}
