using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập Email")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
            public string Password { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role == "Admin") return RedirectToPage("/Admin/Dashboard");
                if (role == "Landlord") return RedirectToPage("/Landlord/Dashboard");
                return RedirectToPage("/Tenant/Dashboard");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var loginDto = new LoginDto
            {
                Email = Input.Email,
                Password = Input.Password
            };

            var result = await _userService.LoginAsync(loginDto);

            if (!result.IsSuccess)
            {
                ErrorMessage = !string.IsNullOrEmpty(result.Message) 
                    ? result.Message 
                    : result.Errors?.FirstOrDefault() ?? "Đăng nhập thất bại. Vui lòng thử lại.";
                return Page();
            }

            var user = result.Data;
            
            // Determine the active role for the session
            string userRole = "Tenant";
            if (user.Role == UserRole.Admin)
            {
                userRole = "Admin";
            }
            else if (user.IsLandlord)
            {
                userRole = "Landlord";
            }
            else if (user.IsTenant)
            {
                userRole = "Tenant";
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, userRole),
                new Claim("AvatarUrl", user.AvatarUrl ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (userRole == "Admin") return RedirectToPage("/Admin/Dashboard");
            if (userRole == "Landlord") return RedirectToPage("/Landlord/Dashboard");
            return RedirectToPage("/Tenant/Dashboard");
        }
    }
}
