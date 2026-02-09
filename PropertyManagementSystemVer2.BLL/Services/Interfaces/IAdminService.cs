using PropertyManagementSystemVer2.BLL.DTOs;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IAdminService
    {
        // User Management
        Task<ServiceResultDto<List<AdminUserDto>>> GetAllUsersAsync();
        Task<ServiceResultDto<AdminUserDto>> GetUserByIdAsync(int userId);
        Task<ServiceResultDto> ActivateDeactivateUserAsync(int userId, bool isActive);
        Task<ServiceResultDto> ResetUserPasswordAsync(int userId);
        Task<ServiceResultDto> UpdateUserRolesAsync(int userId, bool isTenant, bool isLandlord);

        // System Configuration
        Task<ServiceResultDto<List<SystemConfigurationDto>>> GetAllConfigurationsAsync();
        Task<ServiceResultDto> UpdateConfigurationAsync(int adminId, UpdateSystemConfigDto dto);

        // Property Moderation
        Task<ServiceResultDto> ForceUnpublishPropertyAsync(int adminId, int propertyId, string reason);
    }
}
