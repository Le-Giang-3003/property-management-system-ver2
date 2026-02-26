using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        Task<Property?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Property>> GetByLandlordIdAsync(int landlordId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Property>> GetByStatusAsync(PropertyStatus status, CancellationToken cancellationToken = default);
        Task<IEnumerable<Property>> SearchAsync(string? keyword, PropertyType? propertyType, PropertyStatus? status, decimal? minPrice, decimal? maxPrice, string? city, string? district, int? minBedrooms, int? minArea, int pageNumber, int pageSize, string? sortBy, CancellationToken cancellationToken = default);
        Task<int> CountSearchAsync(string? keyword, PropertyType? propertyType, PropertyStatus? status, decimal? minPrice, decimal? maxPrice, string? city, string? district, int? minBedrooms, int? minArea, CancellationToken cancellationToken = default);
        Task<bool> HasActiveLeaseAsync(int propertyId, CancellationToken cancellationToken = default);
        Task<bool> HasPendingPaymentAsync(int propertyId, CancellationToken cancellationToken = default);
    }
}
