using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PropertyManagementSystemVer2.BLL.External;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class SmsService : ISmsService
    {
        private readonly SmsSettings _settings;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IOptions<SmsSettings> settings, ILogger<SmsService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendOtpAsync(string phoneNumber, string otpCode)
        {
            var message = $"[PropertyMS] Mã OTP của bạn là: {otpCode}. Có hiệu lực 30 phút. Không chia sẻ mã này.";
            await SendSmsAsync(phoneNumber, message);
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            // Chuẩn hóa SĐT VN → format quốc tế
            var formattedPhone = FormatPhoneNumber(phoneNumber);

            switch (_settings.Provider.ToLower())
            {
                case "twilio":
                    await SendViaTwilioAsync(formattedPhone, message);
                    break;

                case "console":
                default:
                    // Dev mode: in OTP ra console thay vì gửi thật
                    _logger.LogWarning(
                        "=== SMS (Console Mode) ===\nTo: {Phone}\nMessage: {Message}\n==========================",
                        formattedPhone, message);
                    break;
            }
        }

        // =================================================================
        // TWILIO
        // =================================================================
        private async Task SendViaTwilioAsync(string phoneNumber, string message)
        {
            try
            {
                TwilioClient.Init(_settings.TwilioAccountSid, _settings.TwilioAuthToken);

                var result = await MessageResource.CreateAsync(
                    to: new PhoneNumber(phoneNumber),
                    from: new PhoneNumber(_settings.TwilioPhoneNumber),
                    body: message
                );

                _logger.LogInformation(
                    "SMS sent via Twilio to {Phone} - SID: {Sid}, Status: {Status}",
                    phoneNumber, result.Sid, result.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS via Twilio to {Phone}", phoneNumber);
                throw new InvalidOperationException($"Gửi SMS thất bại: {ex.Message}", ex);
            }
        }

        // =================================================================
        // CHUẨN HÓA SĐT VN
        // =================================================================
        private static string FormatPhoneNumber(string phone)
        {
            phone = phone.Trim().Replace(" ", "").Replace("-", "");

            // 0901234567 → +84901234567
            if (phone.StartsWith("0") && phone.Length == 10)
                return "+84" + phone[1..];

            // 84901234567 → +84901234567
            if (phone.StartsWith("84") && phone.Length == 11)
                return "+" + phone;

            // Đã có +84 hoặc format quốc tế khác
            if (phone.StartsWith("+"))
                return phone;

            return phone;
        }
    }
}
