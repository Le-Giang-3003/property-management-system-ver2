using Microsoft.Extensions.Configuration;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class DataSeedService : IDataSeedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        private const string ConfigKeyAdminEmail = "Seed:AdminEmail";
        private const string ConfigKeyAdminPassword = "Seed:AdminPassword";
        private const string DefaultAdminEmail = "admin@localhost";
        private const string DefaultAdminPassword = "Admin@123";

        public DataSeedService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task EnsureSeedDataAsync(CancellationToken ct = default)
        {
            // Nếu đã có ít nhất một user với role Admin thì không seed thêm
            var (_, adminCount) = await _unitOfWork.Users.GetPagedAsync(
                search: null, role: UserRole.Admin, isActive: null, isLandlord: null, page: 1, pageSize: 1, ct);
            if (adminCount > 0)
                return;

            var email = _configuration[ConfigKeyAdminEmail] ?? DefaultAdminEmail;
            var password = _configuration[ConfigKeyAdminPassword] ?? DefaultAdminPassword;

            if (await _unitOfWork.Users.GetByEmailAsync(email.Trim(), ct) != null)
                return;

            var admin = new User
            {
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                FullName = "Administrator",
                PhoneNumber = "0900000000",
                Role = UserRole.Admin,
                IsActive = true,
                IsTenant = false,
                IsLandlord = false,
                IsEmailVerified = true,
                IsPhoneVerified = false,
                IsIdentityVerified = false,
                LandlordStatus = LandlordApprovalStatus.None,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(admin, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
