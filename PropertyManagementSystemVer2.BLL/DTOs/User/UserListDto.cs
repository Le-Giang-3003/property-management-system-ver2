using PropertyManagementSystemVer2.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.DTOs.User
{
    public class UserListDto
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
}
