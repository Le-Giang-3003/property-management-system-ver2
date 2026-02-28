using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly IUserService _userService;

        public UsersModel(IUserService userService)
        {
            _userService = userService;
        }

        public List<UserDto> Users { get; set; } = new List<UserDto>();
        public List<UserDto> FilteredUsers { get; set; } = new List<UserDto>();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string RoleFilter { get; set; } = "All";

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await _userService.GetAllUsersAsync();
            if (result.IsSuccess && result.Data != null)
            {
                Users = result.Data.OrderByDescending(u => u.CreatedAt).ToList();

                // Apply Filters
                var query = Users.AsQueryable();

                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    query = query.Where(u => u.FullName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) || 
                                             u.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
                }

                if (RoleFilter != "All")
                {
                    if (RoleFilter == "Admin")
                        query = query.Where(u => u.Role.ToString() == "Admin");
                    else if (RoleFilter == "Landlord")
                        query = query.Where(u => u.IsLandlord);
                    else if (RoleFilter == "Tenant")
                        query = query.Where(u => u.IsTenant && !u.IsLandlord);
                }

                FilteredUsers = query.ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(int userId, bool currentStatus)
        {
            var result = await _userService.ActivateDeactivateUserAsync(userId, !currentStatus);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToPage(new { SearchTerm, RoleFilter });
        }
    }
}
