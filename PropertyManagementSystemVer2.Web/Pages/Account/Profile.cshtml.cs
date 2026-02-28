using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IPropertyService _propertyService;
        private readonly IPhotoService _photoService;

        public ProfileModel(IUserService userService, IPropertyService propertyService, IPhotoService photoService)
        {
            _userService = userService;
            _propertyService = propertyService;
            _photoService = photoService;
        }

        [BindProperty]
        public IFormFile? AvatarFile { get; set; }

        [BindProperty]
        public UserDto UserProfile { get; set; } = new();

        [BindProperty]
        public UpdateProfileDto UpdateForm { get; set; } = new();

        [BindProperty]
        public ChangePasswordViewModel PasswordForm { get; set; } = new();

        public PropertySummaryDto LandlordStats { get; set; } = new();

        public string UserRole { get; set; } = string.Empty;

        public class ChangePasswordViewModel
        {
            public string CurrentPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return RedirectToPage("/Account/Login");

            UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Tenant";

            var userResult = await _userService.GetByIdAsync(userId);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToPage("/Index");
            }

            UserProfile = userResult.Data;
            
            // Populate UpdateForm
            UpdateForm = new UpdateProfileDto
            {
                FullName = UserProfile.FullName,
                PhoneNumber = UserProfile.PhoneNumber,
                Address = UserProfile.Address ?? "",
                AvatarUrl = UserProfile.AvatarUrl ?? "",
                IdentityNumber = UserProfile.IdentityNumber ?? "",
                BankAccountNumber = UserProfile.BankAccountNumber ?? "",
                BankName = UserProfile.BankName ?? "",
                BankAccountHolder = UserProfile.BankAccountHolder ?? ""
            };

            if (UserRole == "Landlord")
            {
                var statsResult = await _propertyService.GetPropertySummaryAsync(userId);
                if (statsResult.IsSuccess && statsResult.Data != null)
                {
                    LandlordStats = statsResult.Data;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return RedirectToPage("/Account/Login");

            var result = await _userService.UpdateProfileAsync(userId, UpdateForm);
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

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return RedirectToPage("/Account/Login");

            if (string.IsNullOrEmpty(PasswordForm.CurrentPassword) || string.IsNullOrEmpty(PasswordForm.NewPassword))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ mật khẩu hiện tại và mật khẩu mới.";
                return RedirectToPage();
            }

            if (PasswordForm.NewPassword != PasswordForm.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToPage();
            }

            var dto = new ChangePasswordDto 
            {
                CurrentPassword = PasswordForm.CurrentPassword,
                NewPassword = PasswordForm.NewPassword
            };

            var result = await _userService.ChangePasswordAsync(userId, dto);
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

        public async Task<IActionResult> OnPostUploadAvatarAsync()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return RedirectToPage("/Account/Login");

            if (AvatarFile != null)
            {
                using var stream = AvatarFile.OpenReadStream();
                // Dùng thư viện Cloudinary thông qua IPhotoService
                var uploadResult = await _photoService.AddPhotoAsync(stream, AvatarFile.FileName);
                
                if (uploadResult != null)
                {
                    var updateDto = new UpdateProfileDto { AvatarUrl = uploadResult };
                    var result = await _userService.UpdateProfileAsync(userId, updateDto);
                    
                    if (result.IsSuccess)
                    {
                        TempData["SuccessMessage"] = "Cập nhật ảnh đại diện thành công.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Cập nhật ảnh đại diện vào hệ thống thất bại.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Tải ảnh lên Cloudinary thất bại.";
                }
            }

            return RedirectToPage();
        }
    }
}
