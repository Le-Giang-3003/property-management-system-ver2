using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface ISystemConfigurationRepository : IGenericRepository<SystemConfiguration>
    {
        Task<SystemConfiguration?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
        Task<IEnumerable<SystemConfiguration>> GetByTypeAsync(ConfigurationType type, CancellationToken cancellationToken = default);
        Task<IEnumerable<SystemConfiguration>> GetActiveConfigurationsAsync(CancellationToken cancellationToken = default);
    }
}
