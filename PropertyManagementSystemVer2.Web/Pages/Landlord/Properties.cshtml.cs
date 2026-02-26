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
    }
}
