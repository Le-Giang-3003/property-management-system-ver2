using Microsoft.EntityFrameworkCore;
using PropertyManagementSystemVer2.DAL.Data;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.DAL.Repositories.Implementations
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Message>> GetByConversationIdAsync(int conversationId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(m => m.Sender)
                .Include(m => m.ReplyToMessage)
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Message>> SearchInConversationAsync(int conversationId, string keyword, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(m => m.Sender)
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted && m.Content.Contains(keyword))
                .OrderByDescending(m => m.SentAt)
                .ToListAsync(cancellationToken);
        }

        public async Task MarkAsReadAsync(int conversationId, int userId, CancellationToken cancellationToken = default)
        {
            var unreadMessages = await _dbSet
                .Where(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead)
                .ToListAsync(cancellationToken);

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
            }
        }
    }
}
