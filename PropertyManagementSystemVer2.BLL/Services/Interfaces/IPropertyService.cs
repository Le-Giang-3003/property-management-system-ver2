using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<ServiceResultDto<PropertyDetailDto>> CreatePropertyAsync(int landlordId, CreatePropertyDto dto);
        Task<ServiceResultDto<PropertyDetailDto>> UpdatePropertyAsync(int userId, int propertyId, UpdatePropertyDto dto);
        Task<ServiceResultDto<PropertyDetailDto>> GetByIdAsync(int propertyId);
        Task<ServiceResultDto<PagedResultDto<PropertyListDto>>> SearchPropertiesAsync(PropertySearchDto searchDto);
        Task<ServiceResultDto<List<PropertyListDto>>> GetByLandlordIdAsync(int landlordId, PropertyStatus? status = null);
        Task<ServiceResultDto<List<PropertyListDto>>> GetPendingApprovalAsync();
        Task<ServiceResultDto> ApproveRejectPropertyAsync(int adminId, ApproveRejectPropertyDto dto);
        Task<ServiceResultDto> SubmitForApprovalAsync(int landlordId, int propertyId);
        Task<ServiceResultDto> UnpublishPropertyAsync(int userId, int propertyId);
        Task<ServiceResultDto> SoftDeletePropertyAsync(int userId, int propertyId);
        Task<ServiceResultDto<PropertySummaryDto>> GetPropertySummaryAsync(int landlordId);
        // Image management
        Task<ServiceResultDto<PropertyImageDto>> AddImageAsync(int landlordId, int propertyId, string imageUrl, string? caption, bool isPrimary);
        Task<ServiceResultDto> RemoveImageAsync(int landlordId, int propertyId, int imageId);
        Task<ServiceResultDto> ReorderImagesAsync(int landlordId, int propertyId, List<int> imageIds);
    }
}
