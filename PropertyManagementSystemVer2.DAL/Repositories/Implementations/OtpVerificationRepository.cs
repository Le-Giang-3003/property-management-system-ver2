using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Data;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class OtpVerificationRepository : GenericRepository<OtpVerification>, IOtpVerificationRepository
    {
        public OtpVerificationRepository(AppDbContext context) : base(context) { }

        public async Task<OtpVerification?> GetValidOtpAsync(string otpCode, string purpose)
        {
            return await _dbSet.FirstOrDefaultAsync(o =>
                o.OtpCode == otpCode.Trim() &&
                o.Purpose == purpose &&
                !o.IsUsed &&
                o.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<OtpVerification?> GetValidPhoneOtpAsync(int userId, string phoneNumber, string otpCode)
        {
            return await _dbSet.FirstOrDefaultAsync(o =>
                o.UserId == userId &&
                o.Identifier == phoneNumber.Trim() &&
                o.OtpCode == otpCode.Trim() &&
                o.Purpose == "PhoneVerify" &&
                !o.IsUsed &&
                o.ExpiresAt > DateTime.UtcNow);
        }

        public async Task CleanupExpiredAsync()
        {
            var expired = await _dbSet
                .Where(o => o.ExpiresAt < DateTime.UtcNow && !o.IsUsed)
                .ToListAsync();

            _dbSet.RemoveRange(expired);
        }
    }
}
