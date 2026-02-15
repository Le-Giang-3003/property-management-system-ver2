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
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context) { }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbSet.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserAsync(int userId)
        {
            return await _dbSet
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task RevokeAllByUserAsync(int userId, string? ipAddress = null)
        {
            var activeTokens = await GetActiveTokensByUserAsync(userId);
            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
                _dbSet.Update(token);
            }
        }

        public async Task CleanupExpiredAsync()
        {
            var expired = await _dbSet
                .Where(rt => rt.ExpiresAt < DateTime.UtcNow && rt.RevokedAt == null)
                .ToListAsync();

            foreach (var token in expired)
            {
                token.RevokedAt = DateTime.UtcNow;
                _dbSet.Update(token);
            }
        }
    }
}
