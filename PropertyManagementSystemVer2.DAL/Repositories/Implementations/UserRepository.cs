using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower());
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phone.Trim());
        }

        public async Task<User?> GetByEmailOrPhoneAsync(string identifier)
        {
            var trimmed = identifier.Trim();
            return await _dbSet.FirstOrDefaultAsync(u =>
                u.Email == trimmed.ToLower() || u.PhoneNumber == trimmed);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email.Trim().ToLower());
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _dbSet.AnyAsync(u => u.PhoneNumber == phone.Trim());
        }

        // Landlord registration
        public async Task<IEnumerable<User>> GetPendingLandlordsAsync(int page, int pageSize)
        {
            return await _dbSet
                .Where(u => u.LandlordStatus == LandlordApprovalStatus.Pending)
                .OrderBy(u => u.LandlordSubmittedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountPendingLandlordsAsync()
        {
            return await _dbSet.CountAsync(u => u.LandlordStatus == LandlordApprovalStatus.Pending);
        }

        // Admin user list
        public async Task<(IEnumerable<User> Users, int Total)> GetUsersPagedAsync(
            int page, int pageSize,
            UserRole? roleFilter = null,
            bool? isActiveFilter = null,
            string? search = null)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (roleFilter.HasValue)
                query = query.Where(u => u.Role == roleFilter.Value);

            if (isActiveFilter.HasValue)
                query = query.Where(u => u.IsActive == isActiveFilter.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(u =>
                    u.Email.Contains(s) ||
                    u.FullName.ToLower().Contains(s) ||
                    u.PhoneNumber.Contains(s));
            }

            var total = await query.CountAsync();
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, total);
        }
    }
}
