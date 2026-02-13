using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PropertyManagementSystemVer2.BLL.External;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

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

        // GỬI EMAIL XÁC THỰC TÀI KHOẢN
        public async Task SendVerificationEmailAsync(string toEmail, string fullName, string token)
        {
            var verifyUrl = $"{_settings.BaseUrl}/Auth/VerifyEmail?token={token}";

            var html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'/>
                    <style>
                        body {{ font-family: 'Segoe UI', Arial, sans-serif; background: #f4f7fa; margin: 0; padding: 0; }}
                        .container {{ max-width: 560px; margin: 40px auto; background: #fff; border-radius: 12px; box-shadow: 0 2px 12px rgba(0,0,0,0.08); overflow: hidden; }}
                        .header {{ background: #0d6efd; padding: 30px; text-align: center; }}
                        .header h1 {{ color: #fff; margin: 0; font-size: 24px; }}
                        .body {{ padding: 30px; }}
                        .body h2 {{ color: #333; margin-top: 0; }}
                        .body p {{ color: #555; line-height: 1.6; }}
                        .btn {{ display: inline-block; background: #0d6efd; color: #fff !important; text-decoration: none; padding: 14px 32px; border-radius: 8px; font-weight: 600; font-size: 16px; margin: 20px 0; }}
                        .footer {{ padding: 20px 30px; background: #f8f9fa; text-align: center; font-size: 13px; color: #999; }}
                        .code {{ background: #f0f0f0; padding: 8px 16px; border-radius: 6px; font-family: monospace; font-size: 14px; word-break: break-all; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🏠 PropertyMS</h1>
                        </div>
                        <div class='body'>
                            <h2>Xin chào {fullName},</h2>
                            <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>PropertyMS</strong>.</p>
                            <p>Vui lòng nhấn nút bên dưới để xác thực email của bạn:</p>
                            <div style='text-align: center;'>
                                <a href='{verifyUrl}' class='btn'>✓ Xác thực Email</a>
                            </div>
                            <p>Hoặc copy link sau vào trình duyệt:</p>
                            <p class='code'>{verifyUrl}</p>
                            <p><strong>⏰ Link có hiệu lực trong 24 giờ.</strong></p>
                            <p>Nếu bạn không đăng ký tài khoản này, hãy bỏ qua email này.</p>
                        </div>
                        <div class='footer'>
                            &copy; {DateTime.Now.Year} PropertyMS. All rights reserved.
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, "Xác thực tài khoản PropertyMS", html);
        }

        // GỬI OTP RESET PASSWORD
        public async Task SendPasswordResetOtpAsync(string toEmail, string fullName, string otpCode)
        {
            var html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'/>
                    <style>
                        body {{ font-family: 'Segoe UI', Arial, sans-serif; background: #f4f7fa; margin: 0; padding: 0; }}
                        .container {{ max-width: 560px; margin: 40px auto; background: #fff; border-radius: 12px; box-shadow: 0 2px 12px rgba(0,0,0,0.08); overflow: hidden; }}
                        .header {{ background: #dc3545; padding: 30px; text-align: center; }}
                        .header h1 {{ color: #fff; margin: 0; font-size: 24px; }}
                        .body {{ padding: 30px; }}
                        .body h2 {{ color: #333; margin-top: 0; }}
                        .body p {{ color: #555; line-height: 1.6; }}
                        .otp {{ text-align: center; font-size: 36px; font-weight: 700; letter-spacing: 12px; color: #0d6efd; background: #f0f6ff; padding: 20px; border-radius: 10px; margin: 20px 0; font-family: monospace; }}
                        .footer {{ padding: 20px 30px; background: #f8f9fa; text-align: center; font-size: 13px; color: #999; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🔒 Đặt lại mật khẩu</h1>
                        </div>
                        <div class='body'>
                            <h2>Xin chào {fullName},</h2>
                            <p>Bạn đã yêu cầu đặt lại mật khẩu. Đây là mã OTP của bạn:</p>
                            <div class='otp'>{otpCode}</div>
                            <p><strong>⏰ Mã có hiệu lực 30 phút và chỉ dùng được 1 lần.</strong></p>
                            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này và đảm bảo tài khoản của bạn an toàn.</p>
                        </div>
                        <div class='footer'>
                            &copy; {DateTime.Now.Year} PropertyMS. All rights reserved.
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, "Mã OTP đặt lại mật khẩu - PropertyMS", html);
        }

        // GỬI EMAIL TỔNG QUÁT (SMTP via MailKit)
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // Kết nối SMTP
                await client.ConnectAsync(
                    _settings.SmtpHost,
                    _settings.SmtpPort,
                    _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                // Authenticate
                await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPassword);

                // Gửi
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent to {Email} - Subject: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} - Subject: {Subject}", toEmail, subject);
                throw new InvalidOperationException($"Gửi email thất bại: {ex.Message}", ex);
            }
        }
    }
}