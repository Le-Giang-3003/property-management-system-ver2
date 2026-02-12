using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs.Auth;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystemVer2.Web.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthService _authService;

        public RegisterModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; } = new();

        public class RegisterInputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập họ tên")]
            [MaxLength(200)]
            [Display(Name = "Họ và tên")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập email")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập SĐT")]
            [Phone(ErrorMessage = "SĐT không hợp lệ")]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự")]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
            [Display(Name = "Xác nhận mật khẩu")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Display(Name = "Địa chỉ")]
            public string? Address { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _authService.RegisterAsync(new RegisterRequestDto
            {
                Email = Input.Email,
                Password = Input.Password,
                ConfirmPassword = Input.ConfirmPassword,
                FullName = Input.FullName,
                PhoneNumber = Input.PhoneNumber,
                Address = Input.Address
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
