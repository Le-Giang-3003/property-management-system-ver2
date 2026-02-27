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
                var result = await _leaseService.GetByLandlordIdAsync(userId);
                
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
                            (l.TenantName != null && l.TenantName.ToLower().Contains(lowerSearch)) ||
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

        public async Task<IActionResult> OnPostTerminateAsync(int leaseId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            // Lấy thông tin hợp đồng hiện tại (để pass qua logic service)
            var leaseResult = await _leaseService.GetByIdAsync(leaseId);
            if (!leaseResult.IsSuccess || leaseResult.Data == null)
            {
                TempData["ErrorMessage"] = "Hợp đồng không khả dụng.";
                return RedirectToPage();
            }

            var dto = new EarlyTerminationDto
            {
                LeaseId = leaseId,
                Reason = "Chủ nhà yêu cầu chấm dứt."
            };

            var result = await _leaseService.RequestEarlyTerminationAsync(userId, dto);
            if (result.IsSuccess)
            {
                // Gọi thêm confirm termination luôn nếu logic business cho phép landlord ép chấm dứt đơn phương
                // Ở đây ta gọi RequestEarlyTerminationAsync, service đã đánh dấu LandlordSigned = true
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
