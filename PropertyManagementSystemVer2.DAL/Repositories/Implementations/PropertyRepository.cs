using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(DbContext context) : base(context) { }

        public async Task<Property?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Landlord)
                .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
                .Include(p => p.Leases)
                .Include(p => p.Bookings)
                .Include(p => p.RentalApplications)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Property>> GetByLandlordIdAsync(int landlordId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Images.Where(i => i.IsPrimary))
                .Where(p => p.LandlordId == landlordId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Property>> GetByStatusAsync(PropertyStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Landlord)
                .Include(p => p.Images.Where(i => i.IsPrimary))
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Property>> SearchAsync(string? keyword, PropertyType? propertyType, decimal? minPrice, decimal? maxPrice, string? city, string? district, int? minBedrooms, int? minArea, int pageNumber, int pageSize, string? sortBy, CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .Include(p => p.Landlord)
                .Include(p => p.Images.Where(i => i.IsPrimary))
                .Where(p => p.Status == PropertyStatus.Approved && p.IsAvailable);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.Title.Contains(keyword) || p.Address.Contains(keyword) || p.Description.Contains(keyword));
            }
            if (propertyType.HasValue) query = query.Where(p => p.PropertyType == propertyType.Value);
            if (minPrice.HasValue) query = query.Where(p => p.MonthlyRent >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(p => p.MonthlyRent <= maxPrice.Value);
            if (!string.IsNullOrWhiteSpace(city)) query = query.Where(p => p.City == city);
            if (!string.IsNullOrWhiteSpace(district)) query = query.Where(p => p.District == district);
            if (minBedrooms.HasValue) query = query.Where(p => p.Bedrooms >= minBedrooms.Value);
            if (minArea.HasValue) query = query.Where(p => p.Area >= minArea.Value);

            query = sortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.MonthlyRent),
                "price_desc" => query.OrderByDescending(p => p.MonthlyRent),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                "area" => query.OrderByDescending(p => p.Area),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        }

        public async Task<int> CountSearchAsync(string? keyword, PropertyType? propertyType, decimal? minPrice, decimal? maxPrice, string? city, string? district, int? minBedrooms, int? minArea, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(p => p.Status == PropertyStatus.Approved && p.IsAvailable);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.Title.Contains(keyword) || p.Address.Contains(keyword) || p.Description.Contains(keyword));
            }
            if (propertyType.HasValue) query = query.Where(p => p.PropertyType == propertyType.Value);
            if (minPrice.HasValue) query = query.Where(p => p.MonthlyRent >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(p => p.MonthlyRent <= maxPrice.Value);
            if (!string.IsNullOrWhiteSpace(city)) query = query.Where(p => p.City == city);
            if (!string.IsNullOrWhiteSpace(district)) query = query.Where(p => p.District == district);
            if (minBedrooms.HasValue) query = query.Where(p => p.Bedrooms >= minBedrooms.Value);
            if (minArea.HasValue) query = query.Where(p => p.Area >= minArea.Value);

            return await query.CountAsync(cancellationToken);
        }

        public async Task<bool> HasActiveLeaseAsync(int propertyId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Lease>().AnyAsync(l => l.PropertyId == propertyId && l.Status == LeaseStatus.Active, cancellationToken);
        }

        public async Task<bool> HasPendingPaymentAsync(int propertyId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Payment>()
                .AnyAsync(p => p.Lease.PropertyId == propertyId && (p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Overdue), cancellationToken);
        }
    }
}
