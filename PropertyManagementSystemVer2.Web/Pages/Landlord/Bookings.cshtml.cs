using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class BookingsModel : PageModel
    {
        private readonly IBookingService _bookingService;

        public BookingsModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public List<BookingDto> Bookings { get; set; } = new List<BookingDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _bookingService.GetByLandlordIdAsync(userId);
                if (result.IsSuccess && result.Data != null)
                {
                    Bookings = result.Data;
                }
            }

            return Page();
        }
    }
}
