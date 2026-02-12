using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.Web.Pages.Auth
{
    public class VerifyEmailModel : PageModel
    {
        private readonly IAuthService _authService;

        public VerifyEmailModel(IAuthService authService)
        {
            _authService = authService;
        }

        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string? token = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                IsSuccess = false;
                ErrorMessage = "Link xác thực không hợp lệ.";
                return Page();
            }

            var result = await _authService.VerifyEmailAsync(token);
            IsSuccess = result.Success;
            ErrorMessage = result.Message;

            return Page();
        }
    }
}
