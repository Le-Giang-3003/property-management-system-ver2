using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR1: Dang nhap bang email va password
        // 1. Dang nhap voi email va password
        // 4. JWT token het han sau 60 phut (xu ly o tang Web/Auth middleware)
        public async Task<ServiceResultDto<UserDto>> LoginAsync(LoginDto dto)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResultDto<UserDto>.Failure("Email hoac mat khau khong dung.");

            if (!user.IsActive)
                return ServiceResultDto<UserDto>.Failure("Tai khoan da bi khoa.");

            // Verify password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return ServiceResultDto<UserDto>.Failure("Email hoac mat khau khong dung.");

            // Cap nhat thoi gian dang nhap
            user.LastLoginAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto<UserDto>.Success(MapToUserDto(user));
        }

        // BR2: Dang ky tai khoan moi
        // 1. Dang ky voi email va password
        // 2. Password >= 8 ky tu, gom uppercase, lowercase, number, special char
        // 4. Role mac dinh la Member (Tenant)
        public async Task<ServiceResultDto<UserDto>> RegisterAsync(RegisterDto dto)
        {
            // Kiem tra email da ton tai
            if (await _unitOfWork.Users.EmailExistsAsync(dto.Email))
                return ServiceResultDto<UserDto>.Failure("Email da duoc su dung.");

            // Validate password
            if (!IsValidPassword(dto.Password))
                return ServiceResultDto<UserDto>.Failure("Mat khau phai co it nhat 8 ky tu, bao gom chu hoa, chu thuong, so va ky tu dac biet.");

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Role = UserRole.Member,
                IsTenant = true,
                IsLandlord = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto<UserDto>.Success(MapToUserDto(user), "Dang ky thanh cong.");
        }

        // BR3: Quen/Dat lai mat khau
        // 1. Gui OTP qua mail hoac SDT
        // 2. Link het han sau 30 phut, chi dung 1 lan
        // 3. Invalidate tat ca session cu sau reset
        public async Task<ServiceResultDto> ResetPasswordAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
                return ServiceResultDto.Failure("Khong tim thay tai khoan voi email nay.");

            // Tao mat khau moi tam thoi (thuc te se gui qua email)
            var tempPassword = GenerateTemporaryPassword();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // TODO: Gui email chua mat khau tam thoi hoac link reset
            return ServiceResultDto.Success("Da gui huong dan dat lai mat khau qua email.");
        }

        // BR4: Cap nhat thong tin ca nhan
        // 1. Cap nhat thong tin ca nhan (ten, SDT, avatar, dia chi)
        // 3. Landlord phai bo sung CMND/CCCD, thong tin ngan hang
        public async Task<ServiceResultDto<UserDto>> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ServiceResultDto<UserDto>.Failure("Khong tim thay nguoi dung.");

            if (dto.FullName != null) user.FullName = dto.FullName;
            if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
            if (dto.Address != null) user.Address = dto.Address;
            if (dto.AvatarUrl != null) user.AvatarUrl = dto.AvatarUrl;

            // Cap nhat thong tin Landlord neu co
            if (user.IsLandlord)
            {
                if (dto.IdentityNumber != null) user.IdentityNumber = dto.IdentityNumber;
                if (dto.BankAccountNumber != null) user.BankAccountNumber = dto.BankAccountNumber;
                if (dto.BankName != null) user.BankName = dto.BankName;
                if (dto.BankAccountHolder != null) user.BankAccountHolder = dto.BankAccountHolder;
            }

            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto<UserDto>.Success(MapToUserDto(user), "Cap nhat thong tin thanh cong.");
        }

        public async Task<ServiceResultDto<UserDto>> GetByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ServiceResultDto<UserDto>.Failure("Khong tim thay nguoi dung.");

            return ServiceResultDto<UserDto>.Success(MapToUserDto(user));
        }

        public async Task<ServiceResultDto<UserDto>> GetByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
                return ServiceResultDto<UserDto>.Failure("Khong tim thay nguoi dung.");

            return ServiceResultDto<UserDto>.Success(MapToUserDto(user));
        }

        public async Task<ServiceResultDto> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ServiceResultDto.Failure("Khong tim thay nguoi dung.");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return ServiceResultDto.Failure("Mat khau hien tai khong dung.");

            if (!IsValidPassword(dto.NewPassword))
                return ServiceResultDto.Failure("Mat khau moi phai co it nhat 8 ky tu, bao gom chu hoa, chu thuong, so va ky tu dac biet.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Doi mat khau thanh cong.");
        }

        // BR5: Quan ly Role
        // 1. He thong ho tro dual-role (1 user vua Landlord vua Tenant)
        // 2. Admin quan ly role assignment
        // 3. Role switch khong can logout
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

        public async Task<ServiceResultDto<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var result = users.Select(MapToUserDto).ToList();
            return ServiceResultDto<List<UserDto>>.Success(result);
        }

        public async Task<ServiceResultDto> ActivateDeactivateUserAsync(int userId, bool isActive)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ServiceResultDto.Failure("Khong tim thay nguoi dung.");

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success(isActive ? "Kich hoat tai khoan thanh cong." : "Vo hieu hoa tai khoan thanh cong.");
        }

        // Helper methods
        private static bool IsValidPassword(string password)
        {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;
            if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;
            return true;
        }

        private static string GenerateTemporaryPassword()
        {
            return Guid.NewGuid().ToString("N")[..12] + "A1!";
        }

        private static UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                IsTenant = user.IsTenant,
                IsLandlord = user.IsLandlord,
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified,
                IsIdentityVerified = user.IsIdentityVerified,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}
