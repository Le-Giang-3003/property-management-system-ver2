// File: Pages/Auth/ResetPassword.cshtml.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs.Auth;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.Web.Pages.Auth
{
    public class ResetPasswordModel : PageModel
    {
        private readonly IAuthService _authService;

        public ResetPasswordModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public ResetPasswordInputModel Input { get; set; } = new();

        public class ResetPasswordInputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập mã OTP")]
            [Display(Name = "Mã OTP")]
            public string Token { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
            [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự")]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
            [Display(Name = "Xác nhận mật khẩu")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet(string? identifier = null) { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _authService.ResetPasswordAsync(new ResetPasswordRequestDto
            {
                Token = Input.Token,
                NewPassword = Input.NewPassword,
                ConfirmPassword = Input.ConfirmPassword
            });

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return Page();
            }

            TempData["Success"] = result.Message;
            return RedirectToPage("/Auth/Login");
        }
    }
}