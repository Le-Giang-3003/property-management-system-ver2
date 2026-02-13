using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystemVer2.BLL.DTOs.Auth
{
    public class UpdateProfileRequestDto
    {
        [MaxLength(200)]
        public string? FullName { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
