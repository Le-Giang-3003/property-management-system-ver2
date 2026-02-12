// File: Pages/Auth/ForgotPassword.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs.Auth;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystemVer2.Web.Pages.Auth
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IAuthService _authService;

        public ForgotPasswordModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public ForgotPasswordInputModel Input { get; set; } = new();

        public class ForgotPasswordInputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập email hoặc SĐT")]
            [Display(Name = "Email hoặc SĐT")]
            public string Identifier { get; set; } = string.Empty;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _authService.ForgotPasswordAsync(new ForgotPasswordRequestDto
            {
                Identifier = Input.Identifier
            });

            TempData["Success"] = result.Message;
            return RedirectToPage("/Auth/ResetPassword", new { identifier = Input.Identifier });
        }
    }
}