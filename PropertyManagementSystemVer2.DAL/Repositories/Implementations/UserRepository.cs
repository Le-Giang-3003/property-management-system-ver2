using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(u => u.Role == role).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetLandlordsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(u => u.IsLandlord).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetTenantsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(u => u.IsTenant).ToListAsync(cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
        }
    }
}
