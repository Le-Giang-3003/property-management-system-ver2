using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropertyManagementSystemVer2.BLL.External;
using PropertyManagementSystemVer2.BLL.Identity;
using PropertyManagementSystemVer2.BLL.Security;
using PropertyManagementSystemVer2.BLL.Services.Implementations;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;

namespace PropertyManagementSystemVer2.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services, IConfiguration configuration)
        {
            //settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<SmsSettings>(configuration.GetSection("SmsSettings"));
            //services
            services.AddScoped<IPasswordValidator, PasswordValidator>();
            services.AddScoped<IOtpGenerator, OtpGenerator>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
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

            return services;
        }
    }
}
