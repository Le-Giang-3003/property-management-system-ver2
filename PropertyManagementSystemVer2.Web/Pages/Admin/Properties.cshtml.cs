using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class PropertiesModel : PageModel
    {
        private readonly IPropertyService _propertyService;

        public PropertiesModel(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public List<PropertyListDto> Properties { get; set; } = new List<PropertyListDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            // For demo purposes, we fetch all properties using an empty search
            var searchDto = new PropertySearchDto 
            {
                PageNumber = 1,
                PageSize = 50
            };

            var result = await _propertyService.SearchPropertiesAsync(searchDto);
            
            if (result.IsSuccess && result.Data != null)
            {
                Properties = result.Data.Items.ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int adminId))
            {
                return Unauthorized();
            }

            var dto = new ApproveRejectPropertyDto
            {
                PropertyId = id,
                IsApproved = true
            };

            var result = await _propertyService.ApproveRejectPropertyAsync(adminId, dto);
            
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã duyệt bất động sản thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra khi duyệt.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id, string reason)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int adminId))
            {
                return Unauthorized();
            }

            var dto = new ApproveRejectPropertyDto
            {
                PropertyId = id,
                IsApproved = false,
                RejectionReason = reason
            };

            var result = await _propertyService.ApproveRejectPropertyAsync(adminId, dto);
            
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã từ chối bất động sản thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra khi từ chối.";
            }

            return RedirectToPage();
        }
    }
}
