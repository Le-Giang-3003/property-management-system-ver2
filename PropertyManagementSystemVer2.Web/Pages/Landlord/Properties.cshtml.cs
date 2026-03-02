using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using PropertyManagementSystemVer2.Web.Hubs;

namespace PropertyManagementSystemVer2.Web.Pages.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class PropertiesModel : PageModel
    {
        private readonly IPropertyService _propertyService;
        private readonly IPhotoService _photoService;
        private readonly IHubContext<PropertyHub> _hubContext;

        public PropertiesModel(IPropertyService propertyService, IPhotoService photoService, IHubContext<PropertyHub> hubContext)
        {
            _propertyService = propertyService;
            _photoService = photoService;
            _hubContext = hubContext;
        }

        public List<PropertyListDto> Properties { get; set; } = new List<PropertyListDto>();
        
        public List<PropertyListDto> AllProperties { get; set; } = new List<PropertyListDto>();

        [BindProperty]
        public CreatePropertyDto CreateForm { get; set; } = new CreatePropertyDto();

        [BindProperty]
        public UpdatePropertyDto EditForm { get; set; } = new UpdatePropertyDto();

        [BindProperty]
        public List<IFormFile>? UploadImages { get; set; }

        [BindProperty(SupportsGet = true)]
        public PropertyStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _propertyService.GetByLandlordIdAsync(userId);
                
                if (result.IsSuccess && result.Data != null)
                {
                    AllProperties = result.Data.ToList();
                    var query = result.Data.AsEnumerable();

                    if (StatusFilter.HasValue)
                    {
                        query = query.Where(p => p.Status == StatusFilter.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(SearchTerm))
                    {
                        var term = SearchTerm.Trim().ToLower();
                        query = query.Where(p => 
                            p.Title.ToLower().Contains(term) || 
                            p.Address.ToLower().Contains(term) ||
                            p.City.ToLower().Contains(term));
                    }

                    Properties = query.ToList();
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
                // Real-time broadcast: Get latest list DTO and notify clients
                var latestResult = await _propertyService.GetByIdAsync(id);
                if (latestResult.IsSuccess && latestResult.Data != null)
                {
                    // Map to ListDto for search page
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
                        Landlord = prop.Landlord != null ? new LandlordSummaryDto 
                        { 
                            Id = prop.Landlord.Id, 
                            FullName = prop.Landlord.FullName, 
                            Email = prop.Landlord.Email, 
                            PhoneNumber = prop.Landlord.PhoneNumber 
                        } : null
                    };
                    await _hubContext.Clients.All.SendAsync("PropertyUpdated", listDto);
                }

                TempData["SuccessMessage"] = !string.IsNullOrEmpty(result.Message) ? result.Message : "Cập nhật BDS thành công.";
                return RedirectToPage();
            }

            TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault() ?? "Có lỗi xảy ra khi cập nhật BDS.";
            return await OnGetAsync();
        }

        public async Task<IActionResult> OnPostUploadImagesAsync(int propertyId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            if (UploadImages == null || !UploadImages.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một ảnh.";
                return RedirectToPage();
            }

            int successCount = 0;
            foreach (var file in UploadImages)
            {
                using var stream = file.OpenReadStream();
                var url = await _photoService.AddPhotoAsync(stream, file.FileName);
                if (!string.IsNullOrEmpty(url))
                {
                    // isPrimary is true if no images exist yet (logic handled safely by BLL anyways)
                    var result = await _propertyService.AddImageAsync(userId, propertyId, url, file.FileName, false);
                    if (result.IsSuccess)
                    {
                        successCount++;
                    }
                }
            }

            if (successCount > 0)
            {
                TempData["SuccessMessage"] = $"Đã tải lên thành công {successCount}/{UploadImages.Count} ảnh.";
            }
            else
            {
                TempData["ErrorMessage"] = "Tải ảnh thất bại. Vui lòng kiểm tra lại.";
            }

            return RedirectToPage();
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

        public async Task<IActionResult> OnPostSetPrimaryImageAsync(int propertyId, int imageId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var result = await _propertyService.SetPrimaryImageAsync(userId, propertyId, imageId);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã thay đổi ảnh đại diện thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault() ?? "Lỗi khi thay đổi ảnh đại diện.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostResubmitAsync(int propertyId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var result = await _propertyService.SubmitForApprovalAsync(userId, propertyId);
            if (result.IsSuccess)
            {
                // Real-time broadcast: update Admin view
                var latestResult = await _propertyService.GetByIdAsync(propertyId);
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

                TempData["SuccessMessage"] = "Đã gửi lại yêu cầu duyệt thành công. Vui lòng chờ Admin xác nhận.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault() ?? "Gửi yêu cầu thất bại.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPublishAsync(int propertyId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var result = await _propertyService.PublishPropertyAsync(userId, propertyId);
            if (result.IsSuccess)
            {
                // Real-time broadcast: Show on Tenant search and update Admin
                var latestResult = await _propertyService.GetByIdAsync(propertyId);
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
                        Landlord = prop.Landlord != null ? new LandlordSummaryDto 
                        { 
                            Id = prop.Landlord.Id, 
                            FullName = prop.Landlord.FullName, 
                            Email = prop.Landlord.Email, 
                            PhoneNumber = prop.Landlord.PhoneNumber 
                        } : null
                    };
                    await _hubContext.Clients.All.SendAsync("PropertyUpdated", listDto);
                }

                TempData["SuccessMessage"] = "Đã đăng bài thành công. Khách thuê bây giờ có thể thấy bất động sản này.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault() ?? "Lỗi khi đăng bài.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnpublishAsync(int propertyId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var result = await _propertyService.UnpublishPropertyAsync(userId, propertyId);
            if (result.IsSuccess)
            {
                // Real-time broadcast: Remove from Tenant search and update Admin
                await _hubContext.Clients.All.SendAsync("PropertyDeleted", propertyId);
                
                var latestResult = await _propertyService.GetByIdAsync(propertyId);
                if (latestResult.IsSuccess && latestResult.Data != null)
                {
                    var prop = latestResult.Data;
                    var listDto = new PropertyListDto { Id = prop.Id, Status = prop.Status, Title = prop.Title, MonthlyRent = prop.MonthlyRent, City = prop.City, PropertyType = prop.PropertyType };
                    await _hubContext.Clients.All.SendAsync("PropertyUpdated", listDto);
                }

                TempData["SuccessMessage"] = "Đã tạm ẩn bất động sản. Khách thuê sẽ không còn thấy bài đăng này.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault() ?? "Lỗi khi tạm ẩn bài.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var result = await _propertyService.SoftDeletePropertyAsync(userId, id);
            if (result.IsSuccess)
            {
                // Real-time broadcast: notify clients about deletion
                await _hubContext.Clients.All.SendAsync("PropertyDeleted", id);
                
                TempData["SuccessMessage"] = "Đã xóa bất động sản thành công.";
                return RedirectToPage();
            }

            TempData["ErrorMessage"] = result.Message ?? "Lỗi khi xóa bất động sản.";
            return RedirectToPage();
        }
    }
}
