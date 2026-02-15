using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByPhoneAsync(string phone);
        Task<User?> GetByEmailOrPhoneAsync(string identifier);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default);
        Task<bool> PhoneExistsAsync(string phone);
        Task<IEnumerable<User>> GetPendingLandlordsAsync(int page, int pageSize);
        Task<int> CountPendingLandlordsAsync();
        Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            string? search, UserRole? role, bool? isActive, bool? isLandlord,
            int page, int pageSize, CancellationToken ct = default);

    }
}
