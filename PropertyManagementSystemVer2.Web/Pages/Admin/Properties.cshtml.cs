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

        // Additional POST handlers for Approve/Reject could be added here
    }
}
