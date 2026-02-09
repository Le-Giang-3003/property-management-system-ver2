using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class SystemConfigurationRepository : GenericRepository<SystemConfiguration>, ISystemConfigurationRepository
    {
        public SystemConfigurationRepository(DbContext context) : base(context) { }

        public async Task<SystemConfiguration?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Key == key, cancellationToken);
        }

        public async Task<IEnumerable<SystemConfiguration>> GetByTypeAsync(ConfigurationType type, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(c => c.Type == type).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<SystemConfiguration>> GetActiveConfigurationsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(c => c.IsActive).ToListAsync(cancellationToken);
        }
    }
}
