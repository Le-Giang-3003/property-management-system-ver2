using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.DTOs.User;
using PropertyManagementSystemVer2.BLL.Mapping;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;

        public UserService(IUnitOfWork unitOfWork, IUserRepository userRepo)
        {
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
        }
        public async Task<ServiceResultDto> UpdateUserRoleAsync(UpdateUserRoleDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
            if (user == null)
                return ServiceResultDto.Failure("Khong tim thay nguoi dung.");

            user.IsTenant = dto.IsTenant;
            user.IsLandlord = dto.IsLandlord;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Cap nhat role thanh cong.");
        }

        public async Task<ServiceResultDto<List<UserInfoDto>>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var result = users.Select(UserMapper.ToDto).ToList();
            return ServiceResultDto<List<UserInfoDto>>.Success(result);
        }

        public async Task<PagedResult<UserListDto>> GetUsersPagedAsync(string? search, UserRole? role,
           bool? isActive, bool? isLandlord, int page = 1, int pageSize = 20)
        {
            var (items, totalCount) = await _userRepo.GetPagedAsync(search, role, isActive, isLandlord, page, pageSize);

            return new PagedResult<UserListDto>
            {
                Items = items.Select(u => new UserListDto
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
                }),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<UserDetailDto?> GetUserDetailAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return null;

            var propertyRepo = _unitOfWork.GetRepository<Property>();
            var leaseRepo = _unitOfWork.GetRepository<Lease>();
            var appRepo = _unitOfWork.GetRepository<RentalApplication>();

            return new UserDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role,
                IsActive = user.IsActive,
                IsTenant = user.IsTenant,
                IsLandlord = user.IsLandlord,
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified,
                IsIdentityVerified = user.IsIdentityVerified,
                IdentityNumber = user.IdentityNumber,
                BankAccountNumber = user.BankAccountNumber,
                BankName = user.BankName,
                BankAccountHolder = user.BankAccountHolder,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                OwnedPropertiesCount = await propertyRepo.CountAsync(p => p.LandlordId == userId),
                ActiveLeasesCount = await leaseRepo.CountAsync(l =>
                    (l.LandlordId == userId || l.TenantId == userId) && l.Status == LeaseStatus.Active),
                ApplicationsCount = await appRepo.CountAsync(a => a.TenantId == userId)
            };
        }

        public async Task<bool> UpdateUserAsync(int userId, AdminUpdateUserDto dto, int adminId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            var oldRole = user.Role;
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Role = dto.Role;
            user.IsActive = dto.IsActive;
            user.IsTenant = dto.IsTenant;
            user.IsLandlord = dto.IsLandlord;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            //if (oldRole != dto.Role)
            //{
            //    await _auditLog.LogAsync(adminId, AuditActionType.UserRoleChanged, "User", userId,
            //        $"Role changed from {oldRole} to {dto.Role}");
            //}
            //else
            //{
            //    await _auditLog.LogAsync(adminId, AuditActionType.UserUpdated, "User", userId,
            //        $"User '{user.FullName}' updated by admin");
            //}

            return true;
        }


        public async Task<bool> ToggleActiveAsync(int userId, int adminId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            _userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            //var actionType = user.IsActive ? AuditActionType.UserActivated : AuditActionType.UserDeactivated;
            //await _auditLog.LogAsync(adminId, actionType, "User", userId,
            //    $"User '{user.FullName}' {(user.IsActive ? "activated" : "deactivated")}");

            return true;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword, int adminId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            _userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            //await _auditLog.LogAsync(adminId, AuditActionType.UserPasswordReset, "User", userId,
            //    $"Password reset for user '{user.FullName}'");

            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId, int adminId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            // Soft delete
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            _userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            //await _auditLog.LogAsync(adminId, AuditActionType.UserDeactivated, "User", userId,
            //    $"User '{user.FullName}' deleted (soft) by admin");

            return true;
        }
    }
}
