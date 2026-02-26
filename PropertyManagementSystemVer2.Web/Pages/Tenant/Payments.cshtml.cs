using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Tenant
{
    [Authorize(Roles = "Tenant")]
    public class PaymentsModel : PageModel
    {
        private readonly IPaymentService _paymentService;

        public PaymentsModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _paymentService.GetByTenantIdAsync(userId);
                
                if (result.IsSuccess && result.Data != null)
                {
                    Payments = result.Data;
                }
            }

            return Page();
        }
    }
}
