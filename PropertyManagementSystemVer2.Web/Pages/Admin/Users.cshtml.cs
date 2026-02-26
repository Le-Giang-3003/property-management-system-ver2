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

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await _userService.GetAllUsersAsync();
            if (result.IsSuccess && result.Data != null)
            {
                Users = result.Data;
            }

            return Page();
        }
    }
}
