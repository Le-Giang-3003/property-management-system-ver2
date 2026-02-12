namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string fullName, string token);
        Task SendPasswordResetOtpAsync(string toEmail, string fullName, string otpCode);
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
