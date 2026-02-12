using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs.Auth;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.Web.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;

        public LoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class LoginInputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập email")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _authService.LoginAsync(new LoginRequestDto
            {
                Email = Input.Email,
                Password = Input.Password
            }, HttpContext.Connection.RemoteIpAddress?.ToString());

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return Page();
            }

            var response = result.Data as LoginResponseDto;
            if (response == null)
            {
                TempData["Error"] = "Lỗi hệ thống.";
                return Page();
            }

            HttpContext.Session.SetString("AccessToken", response.AccessToken);
            HttpContext.Session.SetString("RefreshToken", response.RefreshToken);
            HttpContext.Session.SetString("UserId", response.User.Id.ToString());
            HttpContext.Session.SetString("FullName", response.User.FullName);
            HttpContext.Session.SetString("Email", response.User.Email);
            HttpContext.Session.SetString("Role", response.User.Role);
            HttpContext.Session.SetString("IsLandlord", response.User.IsLandlord.ToString());
            HttpContext.Session.SetString("IsTenant", response.User.IsTenant.ToString());
            HttpContext.Session.SetString("AvatarUrl", response.User.AvatarUrl ?? "");
            HttpContext.Session.SetString("CurrentPortal",
                response.User.IsLandlord ? "Landlord" : "Tenant");

            TempData["Success"] = "Đăng nhập thành công!";
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}