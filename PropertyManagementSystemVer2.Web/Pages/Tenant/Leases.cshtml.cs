using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
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

        public List<LeaseDto> AllLeases { get; set; } = new List<LeaseDto>();
        public List<LeaseDto> Leases { get; set; } = new List<LeaseDto>();

        [BindProperty(SupportsGet = true)]
        public LeaseStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _leaseService.GetByTenantIdAsync(userId);
                
                if (result.IsSuccess && result.Data != null)
                {
                    AllLeases = result.Data;

                    var query = AllLeases.AsEnumerable();
                    if (StatusFilter.HasValue)
                    {
                        query = query.Where(l => l.Status == StatusFilter.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(SearchTerm))
                    {
                        var lowerSearch = SearchTerm.ToLower();
                        query = query.Where(l => 
                            (l.LandlordName != null && l.LandlordName.ToLower().Contains(lowerSearch)) ||
                            (l.LeaseNumber != null && l.LeaseNumber.ToLower().Contains(lowerSearch)) ||
                            (l.PropertyTitle != null && l.PropertyTitle.ToLower().Contains(lowerSearch))
                        );
                    }
                    Leases = query.ToList();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSignAsync(int leaseId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var result = await _leaseService.SignLeaseAsync(userId, leaseId);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToPage();
        }
    }
}
