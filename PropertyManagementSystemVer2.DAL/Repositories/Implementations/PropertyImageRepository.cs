using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class PropertyImageRepository : GenericRepository<PropertyImage>, IPropertyImageRepository
    {
        public PropertyImageRepository(DbContext context) : base(context) { }

        public async Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(i => i.PropertyId == propertyId).OrderBy(i => i.DisplayOrder).ToListAsync(cancellationToken);
        }

        public async Task<PropertyImage?> GetPrimaryImageAsync(int propertyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(i => i.PropertyId == propertyId && i.IsPrimary, cancellationToken);
        }

        public async Task<int> CountByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(i => i.PropertyId == propertyId, cancellationToken);
        }
    }
}
