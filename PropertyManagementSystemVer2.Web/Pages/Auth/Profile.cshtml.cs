// File: Pages/Auth/Profile.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs.Auth;
using PropertyManagementSystemVer2.BLL.DTOs.User;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystemVer2.Web.Pages.Auth
{
    public class ProfileModel : PageModel
    {
        private readonly IAuthService _authService;

        public ProfileModel(IAuthService authService)
        {
            _authService = authService;
        }

        public UserInfoDto? Profile { get; set; }

        [BindProperty]
        public ProfileInputModel ProfileInput { get; set; } = new();

        [BindProperty]
        public LandlordInputModel LandlordInput { get; set; } = new();

        [BindProperty]
        public PhoneOtpInputModel PhoneOtpInput { get; set; } = new();

        // === Input Models ===

        public class ProfileInputModel
        {
            [MaxLength(200)]
            [Display(Name = "Họ và tên")]
            public string? FullName { get; set; }

            [Phone]
            [Display(Name = "Số điện thoại")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Địa chỉ")]
            public string? Address { get; set; }

            [Display(Name = "URL Avatar")]
            public string? AvatarUrl { get; set; }
        }

        public class LandlordInputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập số CMND/CCCD")]
            [Display(Name = "Số CMND/CCCD")]
            public string IdentityNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập số tài khoản")]
            [Display(Name = "Số tài khoản")]
            public string BankAccountNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập tên ngân hàng")]
            [Display(Name = "Ngân hàng")]
            public string BankName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập tên chủ tài khoản")]
            [Display(Name = "Chủ tài khoản")]
            public string BankAccountHolder { get; set; } = string.Empty;
        }

        public class PhoneOtpInputModel
        {
            public string PhoneNumber { get; set; } = string.Empty;
            public string OtpCode { get; set; } = string.Empty;
        }

        // === Handlers ===

        private int GetUserId()
        {
            var id = HttpContext.Session.GetString("UserId");
            return int.TryParse(id, out var userId) ? userId : 0;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = GetUserId();
            if (userId == 0) return RedirectToPage("/Auth/Login");

            var result = await _authService.GetProfileAsync(userId);
            if (!result.Success) return RedirectToPage("/Auth/Login");

            Profile = result.Data as UserInfoDto;
            if (Profile != null)
            {
                ProfileInput = new ProfileInputModel
                {
                    FullName = Profile.FullName,
                    PhoneNumber = Profile.PhoneNumber,
                    Address = Profile.Address,
                    AvatarUrl = Profile.AvatarUrl
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var userId = GetUserId();
            if (userId == 0) return RedirectToPage("/Auth/Login");

            var result = await _authService.UpdateProfileAsync(userId, new UpdateProfileRequestDto
            {
                FullName = ProfileInput.FullName,
                PhoneNumber = ProfileInput.PhoneNumber,
                Address = ProfileInput.Address,
                AvatarUrl = ProfileInput.AvatarUrl
            });

            if (result.Success)
            {
                // Update session
                if (!string.IsNullOrEmpty(ProfileInput.FullName))
                    HttpContext.Session.SetString("FullName", ProfileInput.FullName);
                if (!string.IsNullOrEmpty(ProfileInput.AvatarUrl))
                    HttpContext.Session.SetString("AvatarUrl", ProfileInput.AvatarUrl);

                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateLandlordAsync()
        {
            var userId = GetUserId();
            if (userId == 0) return RedirectToPage("/Auth/Login");

            var result = await _authService.UpdateLandlordInfoAsync(userId, new UpdateLandlordInfoRequestDto
            {
                IdentityNumber = LandlordInput.IdentityNumber,
                BankAccountNumber = LandlordInput.BankAccountNumber,
                BankName = LandlordInput.BankName,
                BankAccountHolder = LandlordInput.BankAccountHolder
            });

            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendPhoneOtpAsync()
        {
            var userId = GetUserId();
            if (userId == 0) return RedirectToPage("/Auth/Login");

            var result = await _authService.SendPhoneOtpAsync(userId, new SendPhoneOtpRequestDto
            {
                PhoneNumber = PhoneOtpInput.PhoneNumber
            });

            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostVerifyPhoneOtpAsync()
        {
            var userId = GetUserId();
            if (userId == 0) return RedirectToPage("/Auth/Login");

            var result = await _authService.VerifyPhoneOtpAsync(userId, new VerifyPhoneOtpRequestDto
            {
                PhoneNumber = PhoneOtpInput.PhoneNumber,
                OtpCode = PhoneOtpInput.OtpCode
            });

            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToPage();
        }
    }
}