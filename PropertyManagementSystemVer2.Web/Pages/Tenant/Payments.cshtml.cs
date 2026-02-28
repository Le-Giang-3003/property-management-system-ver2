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
        private readonly IPhotoService _photoService;

        public PaymentsModel(IPaymentService paymentService, IPhotoService photoService)
        {
            _paymentService = paymentService;
            _photoService = photoService;
        }

        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; } = "All";

        [BindProperty]
        public IFormFile? PaymentProofFile { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _paymentService.GetByTenantIdAsync(userId);
                if (result.IsSuccess && result.Data != null)
                {
                    Payments = result.Data.OrderByDescending(p => p.DueDate).ToList();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostMakePaymentAsync(int paymentId, string paymentMethod, string? transactionId, string? notes)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
                return RedirectToPage();

            string? proofUrl = null;

            // Upload bill ảnh lên Cloudinary nếu có
            if (PaymentProofFile != null)
            {
                using var stream = PaymentProofFile.OpenReadStream();
                proofUrl = await _photoService.AddPhotoAsync(stream, PaymentProofFile.FileName);
            }

            // Parse PaymentMethod
            if (!Enum.TryParse<DAL.Enums.PaymentMethod>(paymentMethod, out var method))
                method = DAL.Enums.PaymentMethod.BankTransfer;

            var dto = new MakePaymentDto
            {
                PaymentId = paymentId,
                PaymentMethod = method,
                TransactionId = transactionId,
                PaymentProof = proofUrl,
                Notes = notes
            };

            var result = await _paymentService.MakePaymentAsync(userId, dto);

            if (result.IsSuccess)
                TempData["SuccessMessage"] = "Đã ghi nhận thanh toán. Vui lòng chờ chủ nhà xác nhận.";
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToPage();
        }
    }
}
