using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR64: Quản lý User
        // 1. CRUD users
        // 2. Activate/Deactivate account
        // 3. Reset password
        // 4. Assign/Remove roles
        // 5. View user activity log
        public async Task<ServiceResultDto<List<AdminUserDto>>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var result = users.Select(u => new AdminUserDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                IsActive = u.IsActive,
                IsTenant = u.IsTenant,
                IsLandlord = u.IsLandlord,
                IsEmailVerified = u.IsEmailVerified,
                IsIdentityVerified = u.IsIdentityVerified,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            }).ToList();

            return ServiceResultDto<List<AdminUserDto>>.Success(result);
        }

        public async Task<ServiceResultDto<AdminUserDto>> GetUserByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ServiceResultDto<AdminUserDto>.Failure("Không tìm thấy người dùng.");

            return ServiceResultDto<AdminUserDto>.Success(new AdminUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsActive = user.IsActive,
                IsTenant = user.IsTenant,
                IsLandlord = user.IsLandlord,
                IsEmailVerified = user.IsEmailVerified,
                IsIdentityVerified = user.IsIdentityVerified,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            });
        }

        // BR64.2: Activate/Deactivate
        public async Task<ServiceResultDto> ActivateDeactivateUserAsync(int userId, bool isActive)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ServiceResultDto.Failure("Không tìm thấy người dùng.");

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success(isActive ? "Đã kích hoạt tài khoản." : "Đã vô hiệu hóa tài khoản.");
        }

        // BR64.3: Reset password
        public async Task<ServiceResultDto> ResetUserPasswordAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ServiceResultDto.Failure("Không tìm thấy người dùng.");

            var tempPassword = Guid.NewGuid().ToString("N")[..12] + "A1!";
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // TODO: Gửi mật khẩu mới qua email
            return ServiceResultDto.Success("Đã reset mật khẩu.");
        }

        // BR64.4: Assign/Remove roles
        public async Task<ServiceResultDto> UpdateUserRolesAsync(int userId, bool isTenant, bool isLandlord)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ServiceResultDto.Failure("Không tìm thấy người dùng.");

            user.IsTenant = isTenant;
            user.IsLandlord = isLandlord;
            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Đã cập nhật roles.");
        }

        // BR67: Cấu hình hệ thống
        // 1. Config: late payment fee %, platform fee %, booking slot duration, auto-cancel timeout
        // 2. Moderation on/off
        // 3. Notification templates
        // 4. Lưu audit log mỗi thay đổi config
        public async Task<ServiceResultDto<List<SystemConfigurationDto>>> GetAllConfigurationsAsync()
        {
            var configs = await _unitOfWork.SystemConfigurations.GetActiveConfigurationsAsync();
            var result = configs.Select(c => new SystemConfigurationDto
            {
                Id = c.Id,
                Key = c.Key,
                Value = c.Value,
                Type = c.Type,
                Description = c.Description,
                Unit = c.Unit,
                IsActive = c.IsActive
            }).ToList();

            return ServiceResultDto<List<SystemConfigurationDto>>.Success(result);
        }

        // BR67.1: Update config
        public async Task<ServiceResultDto> UpdateConfigurationAsync(int adminId, UpdateSystemConfigDto dto)
        {
            var config = await _unitOfWork.SystemConfigurations.GetByKeyAsync(dto.Key);
            if (config == null)
                return ServiceResultDto.Failure("Không tìm thấy cấu hình.");

            config.Value = dto.Value;
            if (dto.Description != null) config.Description = dto.Description;
            config.UpdatedBy = adminId;
            config.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.SystemConfigurations.Update(config);
            await _unitOfWork.SaveChangesAsync();

            // BR67.4: TODO - Ghi audit log
            return ServiceResultDto.Success("Đã cập nhật cấu hình.");
        }

        // BR65: Property Moderation - Force unpublish
        // 1. Xem danh sách property Pending Approval, duyệt/reject (handled in PropertyService)
        // 2. Flag/Remove property vi phạm đang Available
        // 4. Force unpublish property nếu phát hiện vi phạm
        public async Task<ServiceResultDto> ForceUnpublishPropertyAsync(int adminId, int propertyId, string reason)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null)
                return ServiceResultDto.Failure("Không tìm thấy property.");

            property.Status = PropertyStatus.Unavailable;
            property.IsAvailable = false;
            property.RejectionReason = $"[Admin Force Unpublish] {reason}";
            property.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Properties.Update(property);

            // Hủy tất cả booking pending
            var pendingBookings = await _unitOfWork.Bookings.GetByPropertyIdAsync(propertyId, BookingStatus.Pending);
            foreach (var booking in pendingBookings)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.CancellationReason = "Property bị admin gỡ do vi phạm.";
                booking.CancelledAt = DateTime.UtcNow;
                _unitOfWork.Bookings.Update(booking);
            }

            await _unitOfWork.SaveChangesAsync();

            // TODO: Notify Landlord
            return ServiceResultDto.Success("Đã force unpublish property.");
        }
    }
}
