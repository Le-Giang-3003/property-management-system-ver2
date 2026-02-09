using PropertyManagementSystemVer2.DAL.Entities;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IPropertyImageRepository : IGenericRepository<PropertyImage>
    {
        Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
        Task<PropertyImage?> GetPrimaryImageAsync(int propertyId, CancellationToken cancellationToken = default);
        Task<int> CountByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
    }
}
