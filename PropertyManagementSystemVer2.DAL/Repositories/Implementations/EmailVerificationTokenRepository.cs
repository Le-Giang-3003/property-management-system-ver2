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
    public class EmailVerificationTokenRepository : GenericRepository<EmailVerificationToken>, IEmailVerificationTokenRepository
    {
        public EmailVerificationTokenRepository(AppDbContext context) : base(context) { }

        public async Task<EmailVerificationToken?> GetValidTokenAsync(string token)
        {
            return await _dbSet.FirstOrDefaultAsync(t =>
                t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task InvalidateAllByUserAsync(int userId)
        {
            var tokens = await _dbSet
                .Where(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsUsed = true;
                token.UsedAt = DateTime.UtcNow;
                _dbSet.Update(token);
            }
        }

        public async Task CleanupExpiredAsync()
        {
            var expired = await _dbSet
                .Where(t => t.ExpiresAt < DateTime.UtcNow && !t.IsUsed)
                .ToListAsync();

            _dbSet.RemoveRange(expired);
        }
    }
}
