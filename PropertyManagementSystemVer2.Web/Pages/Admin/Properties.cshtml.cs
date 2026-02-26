using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;

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

        [BindProperty(SupportsGet = true)]
        public PropertyStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public List<PropertyListDto> Properties { get; set; } = new List<PropertyListDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var searchDto = new PropertySearchDto 
            {
                PageNumber = 1,
                PageSize = 50,
                Status = StatusFilter,
                Keyword = SearchTerm
            };

            var result = await _propertyService.SearchPropertiesAsync(searchDto);
            
            if (result.IsSuccess && result.Data != null)
            {
                var allProperties = result.Data.Items.ToList();
                
                // Filter and sort for Admin view
                Properties = allProperties
                    .OrderBy(p => 
                    {
                        return p.Status switch
                        {
                            PropertyStatus.Pending => 1,
                            PropertyStatus.Approved => 2,
                            PropertyStatus.Rejected => 3,
                            PropertyStatus.Rented => 4,
                            PropertyStatus.Available => 5,
                            PropertyStatus.Unavailable => 6,
                            _ => 7
                        };
                    })
                    .ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetPropertyDetailsAsync(int id)
        {
            var result = await _propertyService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                    PropertyNameCaseInsensitive = true
                };
                return new JsonResult(result.Data, options);
            }
            return new BadRequestObjectResult(result.Message);
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

        public async Task<IActionResult> OnPostBlockAsync(int id, string reason)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int adminId))
            {
                return Unauthorized();
            }

            var result = await _propertyService.BlockPropertyAsync(adminId, id, reason);
            
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã khóa bất động sản thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra khi khóa.";
            }

            return RedirectToPage();
        }
    }
}
