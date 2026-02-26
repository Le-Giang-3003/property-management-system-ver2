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

        public SearchModel(IPropertyService propertyService)
        {
            _propertyService = propertyService;
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
                PageSize = 50
            };

            var result = await _propertyService.SearchPropertiesAsync(searchDto);

            if (result.IsSuccess && result.Data != null)
            {
                Properties = result.Data.Items.ToList();
            }

            return Page();
        }
    }
}
