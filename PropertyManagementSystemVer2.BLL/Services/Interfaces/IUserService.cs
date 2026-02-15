using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.DTOs.User;
using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResultDto> UpdateUserRoleAsync(UpdateUserRoleDto dto);
        Task<ServiceResultDto<List<UserInfoDto>>> GetAllUsersAsync();
        Task<PagedResult<UserListDto>> GetUsersPagedAsync(string? search, UserRole? role,
            bool? isActive, bool? isLandlord, int page = 1, int pageSize = 20);
        Task<UserDetailDto?> GetUserDetailAsync(int userId);
        Task<bool> UpdateUserAsync(int userId, AdminUpdateUserDto dto, int adminId);
        Task<bool> ToggleActiveAsync(int userId, int adminId);
        Task<bool> ResetPasswordAsync(int userId, string newPassword, int adminId);
        Task<bool> DeleteUserAsync(int userId, int adminId);
    }
}
