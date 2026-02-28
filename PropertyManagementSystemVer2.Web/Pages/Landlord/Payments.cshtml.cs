using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class PaymentsModel : PageModel
    {
        private readonly IPaymentService _paymentService;

        public PaymentsModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public List<PaymentDto> Payments { get; set; } = new();
        public List<PaymentDto> FilteredPayments { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; } = "All";

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _paymentService.GetByLandlordIdAsync(userId);
                if (result.IsSuccess && result.Data != null)
                {
                    Payments = result.Data.OrderByDescending(p => p.DueDate).ToList();
                    FilteredPayments = ApplyFilter(Payments);
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmPaymentAsync(int paymentId, bool isConfirmed, string? notes)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
                return RedirectToPage();

            var dto = new ConfirmPaymentDto
            {
                PaymentId = paymentId,
                IsConfirmed = isConfirmed,
                Notes = notes
            };

            var result = await _paymentService.ConfirmPaymentAsync(userId, dto);

            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToPage(new { StatusFilter, SearchTerm });
        }

        private List<PaymentDto> ApplyFilter(List<PaymentDto> all)
        {
            var q = all.AsQueryable();
            if (!string.IsNullOrEmpty(SearchTerm))
                q = q.Where(p => p.TenantName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                 p.LeaseNumber.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

            if (StatusFilter != "All")
                q = q.Where(p => p.Status.ToString() == StatusFilter);

            return q.ToList();
        }
    }
}
