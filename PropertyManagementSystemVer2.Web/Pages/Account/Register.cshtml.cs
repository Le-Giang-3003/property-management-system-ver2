using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
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
        public string ActionType { get; set; } = "SendOtp";

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public class RegisterFormModel
        {
            [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 100 ký tự")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập email")]
            [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [MinLength(8, ErrorMessage = "Mật khẩu phải từ 8 ký tự trở lên")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
            public string PhoneNumber { get; set; } = string.Empty;

            // "Tenant" or "Landlord" — default Tenant
            public string Role { get; set; } = "Tenant";

            public string OtpCode { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            // Ensure default role
            RegisterForm.Role = "Tenant";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // User clicked "Back / change info"
            if (ActionType == "Edit")
            {
                ModelState.Remove("ActionType");
                ActionType = "SendOtp";
                return Page();
            }

            // ===== BƯỚC 1: Validate và gửi OTP =====
            if (ActionType == "SendOtp")
            {
                var validationError = ValidateRegisterForm();
                if (validationError != null)
                {
                    ErrorMessage = validationError;
                    ActionType = "SendOtp";
                    return Page();
                }

                // Chuẩn hóa email NGAY ở đây để cache key nhất quán
                var normalizedEmail = RegisterForm.Email.Trim().ToLower();
                RegisterForm.Email = normalizedEmail;

                // Kiểm tra email chưa tồn tại
                var existingUser = await _userService.GetByEmailAsync(normalizedEmail);
                if (existingUser.IsSuccess && existingUser.Data != null)
                {
                    ErrorMessage = "Email này đã được đăng ký. Vui lòng dùng email khác hoặc đăng nhập.";
                    ActionType = "SendOtp";
                    return Page();
                }

                // Tạo OTP 6 số
                var rnd = new Random();
                var otp = rnd.Next(100000, 999999).ToString();
                System.IO.File.WriteAllText("otp.txt", otp); // FOR TESTING

                // Cache key luôn dùng email đã chuẩn hóa (lowercase, trimmed)
                var cacheKey = $"OTP_{normalizedEmail}";
                _memoryCache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));

                // Gửi email
                var emailBody = BuildOtpEmailBody(RegisterForm.FullName.Trim(), otp);
                try
                {
                    await _emailService.SendEmailAsync(
                        normalizedEmail,
                        "Mã xác thực đăng ký tài khoản PropertyMS",
                        emailBody
                    );
                }
                catch
                {
                    // Xóa OTP nếu gửi email thất bại
                    _memoryCache.Remove(cacheKey);
                    ErrorMessage = "Không thể gửi email xác thực. Vui lòng thử lại.";
                    ActionType = "SendOtp";
                    return Page();
                }

                ModelState.Remove("ActionType");
                ActionType = "Verify";
                SuccessMessage = $"Mã OTP đã được gửi đến {normalizedEmail}. Vui lòng kiểm tra hộp thư (bao gồm thư rác). Mã hết hạn sau 5 phút.";
                return Page();
            }

            // ===== BƯỚC 2: Xác thực OTP và tạo tài khoản =====
            if (ActionType == "Verify")
            {
                // Chuẩn hóa email để cache key khớp với bước SendOtp
                var normalizedEmail = (RegisterForm.Email ?? "").Trim().ToLower();
                var cacheKey = $"OTP_{normalizedEmail}";
                var cleanOtp = (RegisterForm.OtpCode ?? "").Trim();

                // Kiểm tra OTP nhập vào
                if (string.IsNullOrEmpty(cleanOtp) || cleanOtp.Length != 6)
                {
                    ErrorMessage = "Vui lòng nhập đủ 6 chữ số của mã OTP.";
                    ModelState.Remove("ActionType");
                    ActionType = "Verify";
                    return Page();
                }

                // Lấy OTP từ cache
                var cachedOtp = _memoryCache.Get<string>(cacheKey);

                if (cachedOtp == null)
                {
                    ErrorMessage = "Mã OTP đã hết hạn (5 phút). Vui lòng quay lại và gửi lại mã mới.";
                    ModelState.Remove("ActionType");
                    ActionType = "SendOtp";
                    return Page();
                }

                if (cachedOtp != cleanOtp)
                {
                    ErrorMessage = "Mã OTP không chính xác. Vui lòng kiểm tra lại email.";
                    ModelState.Remove("ActionType");
                    ActionType = "Verify";
                    return Page();
                }

                // OTP đúng — Tạo tài khoản
                var registerDto = new RegisterDto
                {
                    FullName    = RegisterForm.FullName.Trim(),
                    Email       = normalizedEmail,
                    Password    = RegisterForm.Password,
                    PhoneNumber = RegisterForm.PhoneNumber.Trim()
                };

                var result = await _userService.RegisterAsync(registerDto);
                if (!result.IsSuccess)
                {
                    // Tài khoản tạo thất bại (VD: email trùng race condition)
                    // Xóa OTP vì không còn dùng được
                    _memoryCache.Remove(cacheKey);
                    ErrorMessage = result.Message;
                    ModelState.Remove("ActionType");
                    ActionType = "SendOtp";
                    return Page();
                }

                // Thành công — xóa OTP khỏi cache
                _memoryCache.Remove(cacheKey);

                // Gán role Landlord nếu được chọn
                if (RegisterForm.Role == "Landlord" && result.Data != null)
                {
                    await _userService.UpdateUserRoleAsync(new UpdateUserRoleDto
                    {
                        UserId     = result.Data.Id,
                        IsTenant   = false,
                        IsLandlord = true
                    });
                }

                ActionType = "Success";
                return Page();
            }

            return Page();
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private string? ValidateRegisterForm()
        {
            if (string.IsNullOrWhiteSpace(RegisterForm.FullName))
                return "Vui lòng nhập họ và tên.";

            if (RegisterForm.FullName.Trim().Length < 2)
                return "Họ tên phải có ít nhất 2 ký tự.";

            if (string.IsNullOrWhiteSpace(RegisterForm.Email))
                return "Vui lòng nhập địa chỉ email.";

            if (!Regex.IsMatch(RegisterForm.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return "Địa chỉ email không hợp lệ.";

            if (string.IsNullOrWhiteSpace(RegisterForm.PhoneNumber))
                return "Vui lòng nhập số điện thoại.";

            var cleanPhone = RegisterForm.PhoneNumber.Replace(" ", "").Replace("-", "");
            if (!Regex.IsMatch(cleanPhone, @"^(0[3|5|7|8|9])[0-9]{8}$"))
                return "Số điện thoại không hợp lệ. Vui lòng nhập số 10 chữ số bắt đầu bằng 0 (VD: 0912345678).";

            RegisterForm.PhoneNumber = cleanPhone;

            if (string.IsNullOrWhiteSpace(RegisterForm.Password))
                return "Vui lòng nhập mật khẩu.";

            if (RegisterForm.Password.Length < 8)
                return "Mật khẩu phải có ít nhất 8 ký tự.";

            // Kiểm tra độ phức tạp mật khẩu (giống yêu cầu của UserService.IsValidPassword)
            if (!RegisterForm.Password.Any(char.IsUpper))
                return "Mật khẩu phải có ít nhất 1 chữ IN HOA (VD: A, B, C...).";

            if (!RegisterForm.Password.Any(char.IsLower))
                return "Mật khẩu phải có ít nhất 1 chữ thường (VD: a, b, c...).";

            if (!RegisterForm.Password.Any(char.IsDigit))
                return "Mật khẩu phải có ít nhất 1 chữ số (VD: 1, 2, 3...).";

            if (!RegisterForm.Password.Any(c => !char.IsLetterOrDigit(c)))
                return "Mật khẩu phải có ít nhất 1 ký tự đặc biệt (VD: @, #, !, *, $...).";

            if (RegisterForm.Password != RegisterForm.ConfirmPassword)
                return "Mật khẩu xác nhận không khớp.";

            if (RegisterForm.Role != "Tenant" && RegisterForm.Role != "Landlord")
                RegisterForm.Role = "Tenant";

            return null; // valid
        }

        private static string BuildOtpEmailBody(string fullName, string otp)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 480px; margin: 0 auto; background: #0f172a; color: #e2e8f0; border-radius: 16px; overflow: hidden;'>
                    <div style='background: linear-gradient(135deg, #6366f1, #8b5cf6); padding: 28px; text-align: center;'>
                        <div style='font-size: 36px; margin-bottom: 8px;'>🏢</div>
                        <h2 style='margin: 0; color: white; font-size: 20px;'>PropertyMS</h2>
                    </div>
                    <div style='padding: 28px;'>
                        <p style='margin: 0 0 16px; font-size: 15px;'>Xin chào <strong style='color: #818cf8;'>{System.Net.WebUtility.HtmlEncode(fullName)}</strong>,</p>
                        <p style='margin: 0 0 24px; color: #94a3b8; font-size: 14px;'>Đây là mã xác thực OTP để hoàn tất đăng ký tài khoản:</p>
                        <div style='background: #1e293b; border: 2px solid #6366f1; border-radius: 12px; padding: 20px; text-align: center; margin-bottom: 24px;'>
                            <div style='font-size: 36px; font-weight: 800; letter-spacing: 12px; color: #818cf8;'>{otp}</div>
                        </div>
                        <p style='margin: 0 0 8px; color: #94a3b8; font-size: 13px;'>⏱ Mã có hiệu lực trong <strong style='color: #f59e0b;'>5 phút</strong>.</p>
                        <p style='margin: 0; color: #94a3b8; font-size: 13px;'>🔒 Không chia sẻ mã này với bất kỳ ai.</p>
                    </div>
                    <div style='background: #0f172a; border-top: 1px solid #1e293b; padding: 16px; text-align: center; font-size: 12px; color: #475569;'>
                        © 2026 PropertyMS — PRN222 Application
                    </div>
                </div>
            ";
        }
    }
}
