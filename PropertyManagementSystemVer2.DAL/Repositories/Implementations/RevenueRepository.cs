using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class RevenueRepository : GenericRepository<Revenue>, IRevenueRepository
    {
        public RevenueRepository(DbContext context) : base(context) { }

        public async Task<IEnumerable<Revenue>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(r => r.PropertyId == propertyId).OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).ToListAsync(cancellationToken);
        }

        public async Task<Revenue?> GetByPropertyAndMonthAsync(int propertyId, int year, int month, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.PropertyId == propertyId && r.Year == year && r.Month == month, cancellationToken);
        }

        public async Task<IEnumerable<Revenue>> GetByYearAsync(int year, int? propertyId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Include(r => r.Property).Where(r => r.Year == year);
            if (propertyId.HasValue) query = query.Where(r => r.PropertyId == propertyId.Value);
            return await query.OrderBy(r => r.Month).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Revenue>> GetByDateRangeAsync(int propertyId, int startYear, int startMonth, int endYear, int endMonth, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(r => r.PropertyId == propertyId &&
                    (r.Year > startYear || (r.Year == startYear && r.Month >= startMonth)) &&
                    (r.Year < endYear || (r.Year == endYear && r.Month <= endMonth)))
                .OrderBy(r => r.Year).ThenBy(r => r.Month)
                .ToListAsync(cancellationToken);
        }
    }
}
