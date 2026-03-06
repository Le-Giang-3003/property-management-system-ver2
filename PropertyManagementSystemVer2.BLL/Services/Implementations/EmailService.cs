using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.BLL.Settings;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Nếu chưa cấu hình email thật → chỉ log ra console (dev mode)
            bool isConfigured = !string.IsNullOrWhiteSpace(_settings.SenderEmail)
                             && _settings.SenderEmail != "your-email@gmail.com"
                             && !string.IsNullOrWhiteSpace(_settings.Password)
                             && _settings.Password != "your-app-password";

            if (!isConfigured)
            {
                _logger.LogWarning("========== [DEV MODE] EMAIL NOT CONFIGURED ==========");
                _logger.LogWarning("Cấu hình EmailSettings trong appsettings.json để gửi email thật.");
                _logger.LogInformation("To     : {To}", toEmail);
                _logger.LogInformation("Subject: {Subject}", subject);
                _logger.LogInformation("Body   : {Body}", body);
                _logger.LogWarning("=====================================================");
                return;
            }

            try
            {
                using var smtpClient = new SmtpClient(_settings.SmtpHost)
                {
                    Port = _settings.SmtpPort,
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password),
                    EnableSsl = _settings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 15000
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(new MailAddress(toEmail));

                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation("Email đã gửi thành công đến {To}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email đến {To}: {Message}", toEmail, ex.Message);
                throw; // Bubble up để Register page hiện thông báo lỗi
            }
        }

        /// <summary>
        /// Gửi email hóa đơn tiền thuê hàng tháng cho Tenant.
        /// HTML template được đóng gói tại đây trong BLL — Web layer không cần biết chi tiết template.
        /// </summary>
        public async Task SendMonthlyInvoiceEmailAsync(string toEmail, string tenantName, string propertyTitle,
                                                       decimal amount, DateTime dueDate, int month, int year)
        {
            var subject = $"[PropertyMS] Hóa đơn tiền thuê tháng {month}/{year}";
            var body = BuildMonthlyInvoiceHtml(tenantName, propertyTitle, amount, dueDate, month, year);
            await SendEmailAsync(toEmail, subject, body);
        }

        // ─── Private Helpers ────────────────────────────────────────────────────────

        private static string BuildMonthlyInvoiceHtml(string tenantName, string propertyTitle,
                                                       decimal amount, DateTime dueDate, int month, int year)
        {
            return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'/></head>
<body style='font-family: Arial, sans-serif; background: #f8fafc; padding: 20px;'>
    <div style='max-width: 560px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.08);'>
        <div style='background: linear-gradient(135deg, #6366f1, #8b5cf6); padding: 28px 32px; color: white;'>
            <div style='font-size: 22px; font-weight: 700; margin-bottom: 4px;'>Hóa đơn tiền thuê</div>
            <div style='font-size: 14px; opacity: 0.85;'>Tháng {month}/{year}</div>
        </div>
        <div style='padding: 28px 32px;'>
            <p style='color: #475569; margin: 0 0 20px;'>Xin chào <strong>{tenantName}</strong>,</p>
            <p style='color: #475569; margin: 0 0 20px;'>Hóa đơn tiền thuê tháng <strong>{month}/{year}</strong> đã được tạo. Vui lòng thanh toán đúng hạn.</p>
            <div style='background: #f8fafc; border-radius: 10px; padding: 20px; margin-bottom: 20px;'>
                <div style='display: flex; justify-content: space-between; margin-bottom: 10px;'>
                    <span style='color: #64748b; font-size: 14px;'>Bất động sản</span>
                    <span style='color: #1e293b; font-weight: 600; font-size: 14px;'>{propertyTitle}</span>
                </div>
                <div style='display: flex; justify-content: space-between; margin-bottom: 10px;'>
                    <span style='color: #64748b; font-size: 14px;'>Số tiền</span>
                    <span style='color: #6366f1; font-weight: 800; font-size: 18px;'>{amount:N0}đ</span>
                </div>
                <div style='display: flex; justify-content: space-between;'>
                    <span style='color: #64748b; font-size: 14px;'>Hạn chót</span>
                    <span style='color: #ef4444; font-weight: 700; font-size: 14px;'>{dueDate:dd/MM/yyyy}</span>
                </div>
            </div>
            <a href='#' style='display: block; text-align: center; background: #6366f1; color: white; padding: 14px; border-radius: 8px; text-decoration: none; font-weight: 600;'>Thanh toán ngay</a>
        </div>
        <div style='padding: 16px 32px; border-top: 1px solid #f1f5f9; color: #94a3b8; font-size: 12px; text-align: center;'>
            PropertyMS - Hệ thống Quản lý Bất động sản
        </div>
    </div>
</body>
</html>";
        }
    }
}
