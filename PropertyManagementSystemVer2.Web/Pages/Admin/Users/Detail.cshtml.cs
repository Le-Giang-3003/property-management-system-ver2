using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs.User;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.Web.Pages.Admin.Users
{
    public class DetailModel : PageModel
    {
        private readonly IUserService _userService;
        public UserDetailDto? User { get; set; }

        public DetailModel(IUserService userService) { _userService = userService; }

        public async Task OnGetAsync(int id)
        {
            User = await _userService.GetUserDetailAsync(id);
            ViewData["AdminName"] = HttpContext.Session.GetString("FullName") ?? "Admin";
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(int id)
        {
            var adminId = HttpContext.Session.GetInt32("UserId") ?? 0;
            await _userService.ToggleActiveAsync(id, adminId);
            TempData["Success"] = "User status updated.";
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(int id, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return RedirectToPage(new { id });
            }
            var adminId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var ok = await _userService.ResetPasswordAsync(id, newPassword, adminId);
            TempData[ok ? "Success" : "Error"] = ok ? "Password reset successfully." : "Failed to reset password.";
            return RedirectToPage(new { id });
        }
    }
}