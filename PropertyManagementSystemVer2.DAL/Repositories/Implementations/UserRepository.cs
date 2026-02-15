using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Data;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower(), ct);
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

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default)
        {
            var query = _dbSet.Where(u => u.Email == email);
            if (excludeId.HasValue)
                query = query.Where(u => u.Id != excludeId.Value);
            return await query.AnyAsync(ct);
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

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            string? search, UserRole? role, bool? isActive, bool? isLandlord,
            int page, int pageSize, CancellationToken ct = default)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(search)
                    || u.Email.ToLower().Contains(search)
                    || u.PhoneNumber.Contains(search));
            }

            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            if (isLandlord.HasValue)
                query = query.Where(u => u.IsLandlord == isLandlord.Value);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }
    }
}
