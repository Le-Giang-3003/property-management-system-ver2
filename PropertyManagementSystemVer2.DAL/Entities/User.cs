using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.DAL.Entities
{
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
        // === LANDLORD REGISTRATION (duyệt bởi Admin) ===
        public LandlordApprovalStatus LandlordStatus { get; set; } = LandlordApprovalStatus.None;
        public string? LandlordRejectionReason { get; set; }    // Lý do Admin reject
        public int? LandlordReviewedBy { get; set; }            // Admin ID
        public DateTime? LandlordReviewedAt { get; set; }
        public DateTime? LandlordSubmittedAt { get; set; }       // Lần submit/resubmit gần nhất
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        // Account lockout
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }

        // Navigation - As Landlord
        public ICollection<Property> OwnedProperties { get; set; } = new List<Property>();
        public ICollection<Lease> LandlordLeases { get; set; } = new List<Lease>();

        // Navigation - As Tenant
        public ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();
        public ICollection<Lease> TenantLeases { get; set; } = new List<Lease>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
