using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.Web.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _memoryCache;

        public RegisterModel(IUserService userService, IEmailService emailService, IMemoryCache memoryCache)
        {
            _userService = userService;
            _emailService = emailService;
            _memoryCache = memoryCache;
        }

        [BindProperty]
        public RegisterFormModel RegisterForm { get; set; } = new();

        [BindProperty]
        public string ActionType { get; set; } = "SendOtp"; // "SendOtp" or "Verify"

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public class RegisterFormModel
        {
            [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập email")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [MinLength(8, ErrorMessage = "Mật khẩu phải từ 8 ký tự")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
            public string PhoneNumber { get; set; } = string.Empty;

            public string Role { get; set; } = "Tenant";

            public string OtpCode { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ActionType == "Edit")
            {
                ActionType = "SendOtp"; // Sets it so next submission will fire SendOtp
                return Page();
            }

            if (ActionType == "SendOtp")
            {
                if (string.IsNullOrEmpty(RegisterForm.Email) || string.IsNullOrEmpty(RegisterForm.Password) || string.IsNullOrEmpty(RegisterForm.FullName) || string.IsNullOrEmpty(RegisterForm.PhoneNumber))
                {
                    ErrorMessage = "Vui lòng điền đầy đủ các thông tin bắt buộc.";
                    return Page();
                }

                if (RegisterForm.Password != RegisterForm.ConfirmPassword)
                {
                    ErrorMessage = "Mật khẩu xác nhận không khớp.";
                    return Page();
                }

                // Generate OTP
                var rnd = new Random();
                var otp = rnd.Next(100000, 999999).ToString();
                
                // Store in cache for 5 minutes
                _memoryCache.Set($"OTP_{RegisterForm.Email}", otp, TimeSpan.FromMinutes(5));

                // Send email
                var emailBody = $"Mã xác thực OTP của bạn là: <b>{otp}</b>. Mã này sẽ hết hạn trong 5 phút.";
                await _emailService.SendEmailAsync(RegisterForm.Email, "Mã xác thực đăng ký tài khoản PropertyMS", emailBody);

                ActionType = "Verify";
                SuccessMessage = "Mã OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư đến (và thư rác). Bạn có 5 phút để nhập mã.";
                return Page();
            }

            if (ActionType == "Verify")
            {
                if (string.IsNullOrEmpty(RegisterForm.OtpCode))
                {
                    ErrorMessage = "Vui lòng nhập mã OTP.";
                    ActionType = "Verify";
                    return Page();
                }

                var cachedOtp = _memoryCache.Get<string>($"OTP_{RegisterForm.Email}");
                if (cachedOtp == null || cachedOtp != RegisterForm.OtpCode)
                {
                    ErrorMessage = "Mã OTP không đúng hoặc đã hết hạn.";
                    ActionType = "Verify";
                    return Page();
                }

                // Remove OTP to prevent reuse
                _memoryCache.Remove($"OTP_{RegisterForm.Email}");

                var registerDto = new RegisterDto
                {
                    FullName = RegisterForm.FullName,
                    Email = RegisterForm.Email,
                    Password = RegisterForm.Password,
                    PhoneNumber = RegisterForm.PhoneNumber
                };

                var result = await _userService.RegisterAsync(registerDto);
                if (!result.IsSuccess)
                {
                    ErrorMessage = result.Message;
                    ActionType = "SendOtp"; // Redirect back to registration if error
                    return Page();
                }

                // Default Role is Tenant in backend. If Landlord is selected, update role.
                if (RegisterForm.Role == "Landlord")
                {
                    await _userService.UpdateUserRoleAsync(new UpdateUserRoleDto {
                        UserId = result.Data.Id,
                        IsTenant = false,
                        IsLandlord = true
                    });
                }

                SuccessMessage = "Đăng ký thành công! Vui lòng đăng nhập.";
                ActionType = "Success";
                return Page();
            }

            return Page();
        }
    }
}
