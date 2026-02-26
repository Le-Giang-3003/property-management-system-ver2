using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class PropertiesModel : PageModel
    {
        private readonly IPropertyService _propertyService;

        public PropertiesModel(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public List<PropertyListDto> Properties { get; set; } = new List<PropertyListDto>();

        [BindProperty]
        public CreatePropertyDto CreateForm { get; set; } = new CreatePropertyDto();

        [BindProperty]
        public UpdatePropertyDto EditForm { get; set; } = new UpdatePropertyDto();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _propertyService.GetByLandlordIdAsync(userId);
                
                if (result.IsSuccess && result.Data != null)
                {
                    Properties = result.Data;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var result = await _propertyService.CreatePropertyAsync(userId, CreateForm);
            
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đăng tin BDS mới thành công.";
                return RedirectToPage();
            }

            TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra khi tạo BDS.";
            return await OnGetAsync();
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var result = await _propertyService.UpdatePropertyAsync(userId, id, EditForm);
            
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Cập nhật BDS thành công.";
                return RedirectToPage();
            }

            TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra khi cập nhật BDS.";
            return await OnGetAsync();
        }
    }
}
