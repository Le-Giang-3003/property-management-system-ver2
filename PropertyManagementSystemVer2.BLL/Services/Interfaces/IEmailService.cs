using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);

        /// <summary>
        /// Gửi email thông báo hóa đơn tiền thuê hàng tháng cho Tenant.
        /// Template HTML được đóng gói trong BLL, Web layer không cần biết.
        /// </summary>
        Task SendMonthlyInvoiceEmailAsync(string toEmail, string tenantName, string propertyTitle,
                                          decimal amount, DateTime dueDate, int month, int year);
    }
}
