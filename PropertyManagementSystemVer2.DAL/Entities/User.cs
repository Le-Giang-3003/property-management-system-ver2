using PropertyManagementSystemVer2.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    //public class User
    //{
    //    public int Id { get; set; }

    //    public string Email { get; set; } = string.Empty;

    //    public string PasswordHash { get; set; } = string.Empty;

    //    public string FullName { get; set; } = string.Empty;

    //    public string PhoneNumber { get; set; } = string.Empty;

    //    public UserRole Role { get; set; }

    //    public string? Address { get; set; }

    //    public string? AvatarUrl { get; set; }

    //    public bool IsActive { get; set; } = true;

    //    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //    public DateTime? UpdatedAt { get; set; }

    //    public DateTime? LastLoginAt { get; set; }

    //    // Navigation properties

    //    /// <summary>
    //    /// Danh sách Properties mà user này là Landlord
    //    /// </summary>
    //    public ICollection<Property> OwnedProperties { get; set; } = new List<Property>();

    //    /// <summary>
    //    /// Danh sách đơn xin thuê mà user này là Tenant
    //    /// </summary>
    //    public ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();

    //    /// <summary>
    //    /// Danh sách hợp đồng thuê mà user này là Tenant
    //    /// </summary>
    //    public ICollection<Lease> TenantLeases { get; set; } = new List<Lease>();

    //    /// <summary>
    //    /// Danh sách hợp đồng thuê mà user này là Landlord
    //    /// </summary>
    //    public ICollection<Lease> LandlordLeases { get; set; } = new List<Lease>();

    //    /// <summary>
    //    /// Danh sách lịch hẹn xem nhà
    //    /// </summary>
    //    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    //}
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Member;
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; } = true;

        // Dual-role capability flags
        public bool IsTenant { get; set; } = true;
        public bool IsLandlord { get; set; } = false;

        // Verification
        public bool IsEmailVerified { get; set; } = false;
        public bool IsPhoneVerified { get; set; } = false;
        public bool IsIdentityVerified { get; set; } = false;

        // Landlord-specific info (required when IsLandlord = true)
        public string? IdentityNumber { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountHolder { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Navigation - As Landlord
        public ICollection<Property> OwnedProperties { get; set; } = new List<Property>();
        public ICollection<Lease> LandlordLeases { get; set; } = new List<Lease>();

        // Navigation - As Tenant
        public ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();
        public ICollection<Lease> TenantLeases { get; set; } = new List<Lease>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
