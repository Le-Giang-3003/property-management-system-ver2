using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Data;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(AppDbContext context) : base(context) { }

        public async Task<Conversation?> GetByIdWithMessagesAsync(int id, int messageCount = 50, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Property)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(messageCount))
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Conversation>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Property)
                .Where(c => (c.User1Id == userId || c.User2Id == userId) && c.IsActive)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Conversation?> GetByUsersAndContextAsync(int user1Id, int user2Id, int? propertyId = null, int? leaseId = null, int? bookingId = null, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(c =>
                ((c.User1Id == user1Id && c.User2Id == user2Id) || (c.User1Id == user2Id && c.User2Id == user1Id)) &&
                c.PropertyId == propertyId && c.LeaseId == leaseId && c.BookingId == bookingId, cancellationToken);
        }
    }
}
