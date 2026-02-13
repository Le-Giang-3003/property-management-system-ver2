using PropertyManagementSystemVer2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IOtpVerificationRepository : IGenericRepository<OtpVerification>
    {
        Task<OtpVerification?> GetValidOtpAsync(string otpCode, string purpose);
        Task<OtpVerification?> GetValidPhoneOtpAsync(int userId, string phoneNumber, string otpCode);
        Task CleanupExpiredAsync();
    }
}
