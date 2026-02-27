using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
