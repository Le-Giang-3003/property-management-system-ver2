using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.DTOs.User;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.Web.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
        private readonly IUserService _userService;
        public UserDetailDto? User { get; set; }
        [BindProperty] public AdminUpdateUserDto Input { get; set; } = new();

        public EditModel(IUserService userService) { _userService = userService; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            User = await _userService.GetUserDetailAsync(id);
            if (User == null) return NotFound();
            Input = new AdminUpdateUserDto
            {
                FullName = User.FullName,
                Email = User.Email,
                PhoneNumber = User.PhoneNumber,
                Role = User.Role,
                IsActive = User.IsActive,
                IsTenant = User.IsTenant,
                IsLandlord = User.IsLandlord
            };
            ViewData["AdminName"] = HttpContext.Session.GetString("FullName") ?? "Admin";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var adminId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var ok = await _userService.UpdateUserAsync(id, Input, adminId);
            if (ok) { TempData["Success"] = "User updated."; return RedirectToPage("/Admin/Users/Detail", new { id }); }
            TempData["Error"] = "Failed to update.";
            User = await _userService.GetUserDetailAsync(id);
            return Page();
        }
    }
}