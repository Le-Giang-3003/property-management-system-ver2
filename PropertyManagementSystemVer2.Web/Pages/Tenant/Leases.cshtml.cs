using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Tenant
{
    [Authorize(Roles = "Tenant")]
    public class LeasesModel : PageModel
    {
        private readonly ILeaseService _leaseService;

        public LeasesModel(ILeaseService leaseService)
        {
            _leaseService = leaseService;
        }

        public List<LeaseDto> Leases { get; set; } = new List<LeaseDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _leaseService.GetByTenantIdAsync(userId);
                
                if (result.IsSuccess && result.Data != null)
                {
                    Leases = result.Data;
                }
            }

            return Page();
        }
    }
}
