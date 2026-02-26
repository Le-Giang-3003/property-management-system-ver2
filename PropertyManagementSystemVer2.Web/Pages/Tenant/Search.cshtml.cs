using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.Web.Pages.Tenant
{
    [Authorize(Roles = "Tenant")]
    public class SearchModel : PageModel
    {
        private readonly IPropertyService _propertyService;
        private readonly IRentalApplicationService _applicationService;

        public SearchModel(IPropertyService propertyService, IRentalApplicationService applicationService)
        {
            _propertyService = propertyService;
            _applicationService = applicationService;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public PropertyType? PropertyType { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinBedrooms { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? City { get; set; }

        [BindProperty]
        public CreateRentalApplicationDto ApplicationForm { get; set; } = new CreateRentalApplicationDto();

        public List<PropertyListDto> Properties { get; set; } = new List<PropertyListDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var searchDto = new PropertySearchDto
            {
                Keyword = SearchKeyword,
                PropertyType = PropertyType,
                MinPrice = MinPrice,
                MaxPrice = MaxPrice,
                MinBedrooms = MinBedrooms,
                City = City,
                PageNumber = 1,
                PageSize = 50,
                Status = PropertyStatus.Available
            };

            var result = await _propertyService.SearchPropertiesAsync(searchDto);

            if (result.IsSuccess && result.Data != null)
            {
                Properties = result.Data.Items.ToList();
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

        public async Task<IActionResult> OnPostApplyAsync(int propertyId)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            // Set the PropertyId to the form
            ApplicationForm.PropertyId = propertyId;

            var result = await _applicationService.SubmitApplicationAsync(userId, ApplicationForm);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã gửi đơn thuê nhà thành công! Vui lòng chờ phản hồi từ Chủ nhà.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra khi nộp đơn thuê.";
            }

            return RedirectToPage();
        }
    }
}
