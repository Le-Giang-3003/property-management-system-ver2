using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.DTOs.User;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.Web.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        public PagedResult<UserListDto> Result { get; set; } = new();

        [BindProperty(SupportsGet = true)] public string? Search { get; set; }
        [BindProperty(SupportsGet = true)] public string? Role { get; set; }
        [BindProperty(SupportsGet = true)] public string? IsActive { get; set; }
        [BindProperty(SupportsGet = true)] public string? IsLandlord { get; set; }
        [BindProperty(SupportsGet = true)] public int CurrentPage { get; set; } = 1;

        public IndexModel(IUserService userService) { _userService = userService; }

        public async Task OnGetAsync()
        {
            UserRole? role = !string.IsNullOrEmpty(Role) ? Enum.Parse<UserRole>(Role) : null;
            bool? isActive = !string.IsNullOrEmpty(IsActive) ? bool.Parse(IsActive) : null;
            bool? isLandlord = !string.IsNullOrEmpty(IsLandlord) ? bool.Parse(IsLandlord) : null;

            Result = await _userService.GetUsersPagedAsync(Search, role, isActive, isLandlord, CurrentPage);
            ViewData["AdminName"] = HttpContext.Session.GetString("FullName") ?? "Admin";
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(int id)
        {
            var adminId = HttpContext.Session.GetInt32("UserId") ?? 0;
            await _userService.ToggleActiveAsync(id, adminId);
            TempData["Success"] = "User status updated.";
            return RedirectToPage();
        }
    }
}