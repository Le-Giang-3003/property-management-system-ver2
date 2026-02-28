using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.Web.Services
{
    /// <summary>
    /// Background service tự động tạo hóa đơn hàng tháng và cập nhật phí quá hạn.
    /// Chạy mỗi 24 giờ (1 lần/ngày).
    /// - Nếu tháng hiện tại chưa có hóa đơn → tạo mới (theo LeaseId, StartDate, EndDate).
    /// - Nếu hóa đơn đã qua DueDate mà chưa thanh toán → cập nhật phí trễ.
    /// - Nếu hóa đơn tháng đó đã có rồi → bỏ qua, không tạo trùng.
    /// </summary>
    public class MonthlyPaymentGeneratorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MonthlyPaymentGeneratorService> _logger;

        // Chạy mỗi 24 tiếng (1 lần/ngày)
        private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(24);

        public MonthlyPaymentGeneratorService(IServiceScopeFactory scopeFactory, ILogger<MonthlyPaymentGeneratorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MonthlyPaymentGeneratorService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in MonthlyPaymentGeneratorService.");
                }

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private async Task RunAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var now = DateTime.UtcNow.AddHours(7); // Vietnam timezone

            // Lấy tất cả Lease đang Active
            var activeLeases = await unitOfWork.Leases.FindAsync(
                l => l.Status == DAL.Enums.LeaseStatus.Active);

            int generated = 0;
            foreach (var lease in activeLeases)
            {
                var result = await paymentService.GenerateMonthlyPaymentsAsync(lease.Id);
                if (result.IsSuccess)
                {
                    generated++;
                    // Gửi email thông báo cho Tenant
                    try
                    {
                        var tenant = lease.Tenant;
                        var dueDate = new DateTime(now.Year, now.Month, lease.PaymentDueDay);
                        var subject = $"[PropertyMS] Hóa đơn tiền thuê tháng {now.Month}/{now.Year}";
                        var body = BuildNewInvoiceEmail(
                            tenant.FullName,
                            lease.Property?.Title ?? "Bất động sản",
                            lease.MonthlyRent,
                            dueDate,
                            now.Month,
                            now.Year
                        );
                        await emailService.SendEmailAsync(tenant.Email, subject, body);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send invoice email for lease {LeaseId}", lease.Id);
                    }
                }
            }

            if (generated > 0)
                _logger.LogInformation("[{Date}] Generated {Count} monthly invoices.", now.ToString("dd/MM/yyyy"), generated);

            // Xử lý các khoản quá hạn
            await paymentService.ProcessOverduePaymentsAsync();
        }

        private static string BuildNewInvoiceEmail(string tenantName, string propertyTitle, decimal amount, DateTime dueDate, int month, int year)
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
