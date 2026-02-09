using PropertyManagementSystemVer2.BLL.DTOs;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResultDto<UserDto>> RegisterAsync(RegisterDto dto);
        Task<ServiceResultDto<UserDto>> LoginAsync(LoginDto dto);
        Task<ServiceResultDto<UserDto>> GetByIdAsync(int userId);
        Task<ServiceResultDto<UserDto>> UpdateProfileAsync(int userId, UpdateProfileDto dto);
        Task<ServiceResultDto> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<ServiceResultDto> UpdateUserRoleAsync(UpdateUserRoleDto dto);
        Task<ServiceResultDto<List<UserDto>>> GetAllUsersAsync();
        Task<ServiceResultDto> ActivateDeactivateUserAsync(int userId, bool isActive);
        Task<ServiceResultDto> ResetPasswordAsync(string email);
    }
}
