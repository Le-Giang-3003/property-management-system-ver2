using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropertyManagementSystemVer2.BLL.Services.Implementations;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.BLL.Settings;

namespace PropertyManagementSystemVer2.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services, IConfiguration? configuration = null)
        {
            // Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IRentalApplicationService, RentalApplicationService>();
            services.AddScoped<ILeaseService, LeaseService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IPhotoService, PhotoService>();

            // Email & Cache (dùng cho OTP đăng ký)
            services.AddScoped<IEmailService, EmailService>();
            services.AddMemoryCache();

            // Email Settings (SMTP)
            if (configuration != null)
                services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            else
                services.Configure<EmailSettings>(_ => { }); // default empty

            return services;
        }
    }
}
