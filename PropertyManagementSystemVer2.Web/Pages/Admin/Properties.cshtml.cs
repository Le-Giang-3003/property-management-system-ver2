using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
using Microsoft.AspNetCore.SignalR;
using PropertyManagementSystemVer2.Web.Hubs;

namespace PropertyManagementSystemVer2.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class PropertiesModel : PageModel
    {
        private readonly IPropertyService _propertyService;
        private readonly IHubContext<PropertyHub> _hubContext;

        public PropertiesModel(IPropertyService propertyService, IHubContext<PropertyHub> hubContext)
        {
            _propertyService = propertyService;
            _hubContext = hubContext;
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
                // Real-time broadcast: Get latest list DTO and notify clients
                var latestResult = await _propertyService.GetByIdAsync(id);
                if (latestResult.IsSuccess && latestResult.Data != null)
                {
                    var prop = latestResult.Data;
                    var listDto = new PropertyListDto
                    {
                        Id = prop.Id,
                        Title = prop.Title,
                        MonthlyRent = prop.MonthlyRent,
                        City = prop.City,
                        District = prop.District,
                        Ward = prop.Ward,
                        Address = prop.Address,
                        ThumbnailUrl = prop.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl ?? prop.Images.FirstOrDefault()?.ImageUrl,
                        PropertyType = prop.PropertyType,
                        Status = prop.Status,
                        Bedrooms = prop.Bedrooms,
                        Bathrooms = prop.Bathrooms,
                        Area = prop.Area,
                        Description = prop.Description,
                        Amenities = prop.Amenities,
                        AllowPets = prop.AllowPets,
                        AllowSmoking = prop.AllowSmoking,
                        CreatedAt = prop.CreatedAt,
                        DepositAmount = prop.DepositAmount,
                        Floors = prop.Floors,
                        Landlord = prop.Landlord != null ? new LandlordSummaryDto { Id = prop.Landlord.Id, FullName = prop.Landlord.FullName, Email = prop.Landlord.Email, PhoneNumber = prop.Landlord.PhoneNumber } : null
                    };
                    await _hubContext.Clients.All.SendAsync("PropertyUpdated", listDto);
                }

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
                // Real-time broadcast: notify clients to remove from search page
                await _hubContext.Clients.All.SendAsync("PropertyDeleted", id);
                
                // Also update Admin view list
                var latestResult = await _propertyService.GetByIdAsync(id);
                if (latestResult.IsSuccess && latestResult.Data != null)
                {
                    var prop = latestResult.Data;
                    var listDto = new PropertyListDto
                    {
                        Id = prop.Id,
                        Title = prop.Title,
                        MonthlyRent = prop.MonthlyRent,
                        City = prop.City,
                        District = prop.District,
                        Ward = prop.Ward,
                        Address = prop.Address,
                        ThumbnailUrl = prop.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl ?? prop.Images.FirstOrDefault()?.ImageUrl,
                        PropertyType = prop.PropertyType,
                        Status = prop.Status,
                        Bedrooms = prop.Bedrooms,
                        Bathrooms = prop.Bathrooms,
                        Area = prop.Area,
                        Description = prop.Description,
                        Amenities = prop.Amenities,
                        AllowPets = prop.AllowPets,
                        AllowSmoking = prop.AllowSmoking,
                        CreatedAt = prop.CreatedAt,
                        DepositAmount = prop.DepositAmount,
                        Floors = prop.Floors,
                        Landlord = prop.Landlord != null ? new LandlordSummaryDto { Id = prop.Landlord.Id, FullName = prop.Landlord.FullName, Email = prop.Landlord.Email, PhoneNumber = prop.Landlord.PhoneNumber } : null
                    };
                    await _hubContext.Clients.All.SendAsync("PropertyUpdated", listDto);
                }

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
                // Real-time broadcast: notify clients to remove from search page
                await _hubContext.Clients.All.SendAsync("PropertyDeleted", id);

                // Also update Admin view list
                var latestResult = await _propertyService.GetByIdAsync(id);
                if (latestResult.IsSuccess && latestResult.Data != null)
                {
                    var prop = latestResult.Data;
                    var listDto = new PropertyListDto
                    {
                        Id = prop.Id,
                        Title = prop.Title,
                        MonthlyRent = prop.MonthlyRent,
                        City = prop.City,
                        District = prop.District,
                        Ward = prop.Ward,
                        Address = prop.Address,
                        ThumbnailUrl = prop.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl ?? prop.Images.FirstOrDefault()?.ImageUrl,
                        PropertyType = prop.PropertyType,
                        Status = prop.Status,
                        Bedrooms = prop.Bedrooms,
                        Bathrooms = prop.Bathrooms,
                        Area = prop.Area,
                        Description = prop.Description,
                        Amenities = prop.Amenities,
                        AllowPets = prop.AllowPets,
                        AllowSmoking = prop.AllowSmoking,
                        CreatedAt = prop.CreatedAt,
                        DepositAmount = prop.DepositAmount,
                        Floors = prop.Floors,
                        Landlord = prop.Landlord != null ? new LandlordSummaryDto { Id = prop.Landlord.Id, FullName = prop.Landlord.FullName, Email = prop.Landlord.Email, PhoneNumber = prop.Landlord.PhoneNumber } : null
                    };
                    await _hubContext.Clients.All.SendAsync("PropertyUpdated", listDto);
                }

                TempData["SuccessMessage"] = "Đã khóa bài đăng bất động sản thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra khi khóa.";
            }

            return RedirectToPage();
        }
    }
}
