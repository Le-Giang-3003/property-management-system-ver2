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
    public class ApplicationsModel : PageModel
    {
        private readonly IRentalApplicationService _rentalApplicationService;

        public ApplicationsModel(IRentalApplicationService rentalApplicationService)
        {
            _rentalApplicationService = rentalApplicationService;
        }

        public List<RentalApplicationDto> AllApplications { get; set; } = new List<RentalApplicationDto>();
        public List<RentalApplicationDto> Applications { get; set; } = new List<RentalApplicationDto>();

        [BindProperty(SupportsGet = true)]
        public ApplicationStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _rentalApplicationService.GetByTenantIdAsync(userId);
                
                if (result.IsSuccess && result.Data != null)
                {
                    AllApplications = result.Data;

                    var query = AllApplications.AsEnumerable();
                    if (StatusFilter.HasValue)
                    {
                        query = query.Where(a => a.Status == StatusFilter.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(SearchTerm))
                    {
                        var lowerSearch = SearchTerm.ToLower();
                        query = query.Where(a => 
                            (a.PropertyTitle != null && a.PropertyTitle.ToLower().Contains(lowerSearch))
                        );
                    }
                    Applications = query.ToList();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostWithdrawAsync(int applicationId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var result = await _rentalApplicationService.WithdrawApplicationAsync(userId, applicationId);
            if(result.IsSuccess) TempData["SuccessMessage"] = result.Message;
            else TempData["ErrorMessage"] = result.Message;

            return RedirectToPage();
        }
    }
}
