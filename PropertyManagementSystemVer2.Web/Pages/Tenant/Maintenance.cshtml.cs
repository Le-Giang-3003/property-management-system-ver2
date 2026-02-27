using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.IO;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Tenant
{
    [Authorize(Roles = "Tenant")]
    public class MaintenanceModel : PageModel
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly ILeaseService _leaseService;

        public MaintenanceModel(IMaintenanceService maintenanceService, ILeaseService leaseService)
        {
            _maintenanceService = maintenanceService;
            _leaseService = leaseService;
        }

        public List<MaintenanceRequestDto> MaintenanceRequests { get; set; } = new List<MaintenanceRequestDto>();
        public List<LeaseDto> ActiveLeases { get; set; } = new List<LeaseDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var result = await _maintenanceService.GetByTenantIdAsync(userId);
                if (result.IsSuccess && result.Data != null)
                {
                    MaintenanceRequests = result.Data;
                }

                var leaseResult = await _leaseService.GetByTenantIdAsync(userId, PropertyManagementSystemVer2.DAL.Enums.LeaseStatus.Active);
                if (leaseResult.IsSuccess && leaseResult.Data != null)
                {
                    ActiveLeases = leaseResult.Data;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync(int leaseId, int propertyId, DAL.Enums.MaintenancePriority priority, DAL.Enums.MaintenanceCategory category, string title, string description, DateTime? scheduledDate, IFormFileCollection files)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var uploadedUrls = new List<string>();
                if (files != null && files.Any())
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "maintenance");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    
                    foreach (var file in files)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        uploadedUrls.Add("/uploads/maintenance/" + uniqueFileName);
                    }
                }

                var dto = new CreateMaintenanceRequestDto
                {
                    LeaseId = leaseId,
                    PropertyId = propertyId,
                    Title = title,
                    Description = description,
                    Category = category,
                    Priority = priority,
                    ImageUrls = uploadedUrls.Any() ? string.Join(",", uploadedUrls) : null,
                    ScheduledDate = scheduledDate
                };

                var result = await _maintenanceService.CreateRequestAsync(userId, dto);
                
                if (!result.IsSuccess)
                {
                    TempData["ErrorMessage"] = result.Message;
                }
                else
                {
                    TempData["SuccessMessage"] = "Đã gửi yêu cầu thành công";
                }
            }
            return RedirectToPage();
        }
    }
}
