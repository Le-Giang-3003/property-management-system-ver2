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
    }
}
