using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByPhoneAsync(string phone);
        Task<User?> GetByEmailOrPhoneAsync(string identifier);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phone);
        Task<IEnumerable<User>> GetPendingLandlordsAsync(int page, int pageSize);
        Task<int> CountPendingLandlordsAsync();
        Task<(IEnumerable<User> Users, int Total)> GetUsersPagedAsync(
            int page, int pageSize,
            UserRole? roleFilter = null,
            bool? isActiveFilter = null,
            string? search = null);
    }
}
