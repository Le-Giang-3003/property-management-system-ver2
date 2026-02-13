using PropertyManagementSystemVer2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IEmailVerificationTokenRepository : IGenericRepository<EmailVerificationToken>
    {
        Task<EmailVerificationToken?> GetValidTokenAsync(string token);
        Task InvalidateAllByUserAsync(int userId);
        Task CleanupExpiredAsync();
    }
}
