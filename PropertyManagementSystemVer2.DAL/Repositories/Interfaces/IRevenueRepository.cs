using PropertyManagementSystemVer2.DAL.Entities;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IRevenueRepository : IGenericRepository<Revenue>
    {
        Task<IEnumerable<Revenue>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
        Task<Revenue?> GetByPropertyAndMonthAsync(int propertyId, int year, int month, CancellationToken cancellationToken = default);
        Task<IEnumerable<Revenue>> GetByYearAsync(int year, int? propertyId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Revenue>> GetByDateRangeAsync(int propertyId, int startYear, int startMonth, int endYear, int endMonth, CancellationToken cancellationToken = default);
    }
}
