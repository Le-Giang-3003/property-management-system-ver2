using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // In a production environment, you would use a real SMTP server.
                // For this demo, we'll log the email content to the console to simulate sending the OTP.
                _logger.LogInformation($"========== EMAIL SENT ==========");
                _logger.LogInformation($"To: {toEmail}");
                _logger.LogInformation($"Subject: {subject}");
                _logger.LogInformation($"Body: {body}");
                _logger.LogInformation($"================================");
                
                await Task.CompletedTask;

                /* Example real SMTP client setup
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("your-email@gmail.com", "your-app-password"),
                    EnableSsl = true,
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("your-email@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                */
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gửi email");
            }
        }
    }
}
