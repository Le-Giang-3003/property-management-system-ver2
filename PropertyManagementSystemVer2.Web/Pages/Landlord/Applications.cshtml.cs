using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class ApplicationsModel : PageModel
    {
        private readonly IRentalApplicationService _rentalApplicationService;
        private readonly ILeaseService _leaseService;

        public ApplicationsModel(IRentalApplicationService rentalApplicationService, ILeaseService leaseService)
        {
            _rentalApplicationService = rentalApplicationService;
            _leaseService = leaseService;
        }

        public List<RentalApplicationDto> AllApplications { get; set; } = new List<RentalApplicationDto>();
        public List<RentalApplicationDto> Applications { get; set; } = new List<RentalApplicationDto>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public ApplicationStatus? StatusFilter { get; set; }

        [BindProperty]
        public CreateLeaseDto CreateLeaseForm { get; set; } = new CreateLeaseDto();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var allResult = await _rentalApplicationService.GetByLandlordIdAsync(userId);
                if (allResult.IsSuccess && allResult.Data != null)
                {
                    AllApplications = allResult.Data;
                    
                    var query = AllApplications.AsEnumerable();
                    if (StatusFilter.HasValue)
                    {
                        query = query.Where(a => a.Status == StatusFilter.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(SearchTerm))
                    {
                        var lowerSearch = SearchTerm.ToLower();
                        query = query.Where(a => 
                            (a.TenantName != null && a.TenantName.ToLower().Contains(lowerSearch)) ||
                            (a.TenantPhone != null && a.TenantPhone.Contains(lowerSearch)) ||
                            (a.PropertyTitle != null && a.PropertyTitle.ToLower().Contains(lowerSearch))
                        );
                    }
                    Applications = query.ToList();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRejectAsync(int applicationId, string reason)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var dto = new ApproveRejectApplicationDto 
            {
                ApplicationId = applicationId,
                IsApproved = false,
                RejectionReason = reason
            };

            var result = await _rentalApplicationService.ApproveRejectApplicationAsync(userId, dto);
            if(result.IsSuccess) TempData["SuccessMessage"] = "Đã từ chối đơn xin thuê.";
            else TempData["ErrorMessage"] = result.Message;

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostApproveAndCreateLeaseAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            // First Approve Application
            var approveDto = new ApproveRejectApplicationDto 
            {
                ApplicationId = CreateLeaseForm.RentalApplicationId,
                IsApproved = true
            };

            var approveResult = await _rentalApplicationService.ApproveRejectApplicationAsync(userId, approveDto);
            if (!approveResult.IsSuccess)
            {
                TempData["ErrorMessage"] = approveResult.Message;
                return RedirectToPage();
            }

            // Then Create Lease
            var leaseResult = await _leaseService.CreateLeaseAsync(userId, CreateLeaseForm);
            if (leaseResult.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã phê duyệt đơn và tạo hợp đồng thành công!";
                return Redirect("/Landlord/Leases");
            }
            else
            {
                TempData["ErrorMessage"] = leaseResult.Message;
                return RedirectToPage();
            }
        }
    }
}
