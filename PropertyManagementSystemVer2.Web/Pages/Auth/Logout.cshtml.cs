using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.Web.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly IAuthService _authService;

        public LogoutModel(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            var refreshToken = HttpContext.Session.GetString("RefreshToken");

            if (int.TryParse(userIdStr, out var userId) && !string.IsNullOrEmpty(refreshToken))
            {
                await _authService.LogoutAsync(userId, refreshToken,
                    HttpContext.Connection.RemoteIpAddress?.ToString());
            }

            HttpContext.Session.Clear();
            TempData["Success"] = "Đăng xuất thành công.";
            return RedirectToPage("/Auth/Login");
        }
    }
}
