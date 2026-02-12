using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class OtpVerification
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Identifier { get; set; } = string.Empty; // Email hoặc SĐT
        public string OtpCode { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty; // "PhoneVerify", "ResetPassword"
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
    }
}
