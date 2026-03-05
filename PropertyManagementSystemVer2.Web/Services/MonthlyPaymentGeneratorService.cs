using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.Web.Services
{
    /// <summary>
    /// Background service tự động tạo hóa đơn hàng tháng và xử lý phí quá hạn.
   
    /// </summary>
    public class MonthlyPaymentGeneratorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MonthlyPaymentGeneratorService> _logger;

        // Chạy mỗi 24 tiếng (1 lần/ngày)
        private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(24);

        // Múi giờ Việt Nam — dùng TimeZoneInfo thay vì hardcode +7
        private static readonly TimeZoneInfo VietnamTz =
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public MonthlyPaymentGeneratorService(
            IServiceScopeFactory scopeFactory,
            ILogger<MonthlyPaymentGeneratorService> logger)
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

            var leaseService   = scope.ServiceProvider.GetRequiredService<ILeaseService>();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            var emailService   = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTz);

            var activeLeases = await leaseService.GetActiveLeasesForBillingAsync();

            int generated = 0;
            foreach (var lease in activeLeases)
            {
                var result = await paymentService.GenerateMonthlyPaymentsAsync(lease.Id);
                if (!result.IsSuccess) continue;

                generated++;

                try
                {
                    var dueDate = new DateTime(now.Year, now.Month,
                        Math.Min(lease.PaymentDueDay, DateTime.DaysInMonth(now.Year, now.Month)));

                    await emailService.SendMonthlyInvoiceEmailAsync(
                        lease.TenantEmail,
                        lease.TenantName,
                        lease.PropertyTitle,
                        lease.MonthlyRent,
                        dueDate,
                        now.Month,
                        now.Year);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send invoice email for lease {LeaseId}.", lease.Id);
                }
            }

            if (generated > 0)
                _logger.LogInformation("[{Date}] Generated {Count} monthly invoices.",
                    now.ToString("dd/MM/yyyy"), generated);

            // ── 4. Xử lý các khoản quá hạn ────────────────────────────────────────
            await paymentService.ProcessOverduePaymentsAsync();
        }
    }
}
