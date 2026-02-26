using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.DTOs
{
    // DTO hiển thị danh sách property (card)
    public class PropertyListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public PropertyType PropertyType { get; set; }
        public PropertyStatus Status { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? Ward { get; set; }
        public decimal Area { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int? Floors { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public string? Amenities { get; set; }
        public bool AllowPets { get; set; }
        public bool AllowSmoking { get; set; }
        public string Currency { get; set; } = "VND";
        public string? ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? RejectionReason { get; set; }
        public LandlordSummaryDto? Landlord { get; set; }
    }

    // DTO hiển thị chi tiết property
    public class PropertyDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PropertyType PropertyType { get; set; }
        public PropertyStatus Status { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? Ward { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal Area { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int? Floors { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public string Currency { get; set; } = "VND";
        public string? Amenities { get; set; }
        public bool AllowPets { get; set; }
        public bool AllowSmoking { get; set; }
        public int? MaxOccupants { get; set; }
        public int LandlordId { get; set; }
        public LandlordSummaryDto? Landlord { get; set; }
        public List<PropertyImageDto> Images { get; set; } = new();
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ViewCount { get; set; }
    }

    // DTO tạo/sửa property
    public class CreatePropertyDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PropertyType PropertyType { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? Ward { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal Area { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int? Floors { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public string? Amenities { get; set; }
        public bool AllowPets { get; set; }
        public bool AllowSmoking { get; set; }
        public int? MaxOccupants { get; set; }
    }

    public class UpdatePropertyDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public PropertyType? PropertyType { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Area { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int? Floors { get; set; }
        public decimal? MonthlyRent { get; set; }
        public decimal? DepositAmount { get; set; }
        public string? Amenities { get; set; }
        public bool AllowPets { get; set; }
        public bool AllowSmoking { get; set; }
        public int? MaxOccupants { get; set; }
    }

    // DTO tìm kiếm property
    public class PropertySearchDto
    {
        public string? Keyword { get; set; }
        public PropertyType? PropertyType { get; set; }
        public PropertyStatus? Status { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public int? MinBedrooms { get; set; }
        public int? MinArea { get; set; }
        public string? SortBy { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // DTO ảnh property
    public class PropertyImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }

    // DTO kết quả phân trang
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    // DTO approve/reject property
    public class ApproveRejectPropertyDto
    {
        public int PropertyId { get; set; }
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }

    // DTO summary cho Landlord Dashboard
    public class PropertySummaryDto
    {
        public int TotalProperties { get; set; }
        public int AvailableCount { get; set; }
        public int RentedCount { get; set; }
        public int PendingCount { get; set; }
        public int InactiveCount { get; set; }
    }
}
