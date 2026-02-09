using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.DTOs
{
    // DTO quản lý user (Admin)
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsTenant { get; set; }
        public bool IsLandlord { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsIdentityVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    // DTO cấu hình hệ thống
    public class SystemConfigurationDto
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public ConfigurationType Type { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateSystemConfigDto
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    // DTO audit log (giả lập - vì chưa có entity AuditLog)
    public class AuditLogDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
