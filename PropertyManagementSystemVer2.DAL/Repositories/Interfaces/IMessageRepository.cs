using PropertyManagementSystemVer2.DAL.Entities;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IEnumerable<Message>> GetByConversationIdAsync(int conversationId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<IEnumerable<Message>> SearchInConversationAsync(int conversationId, string keyword, CancellationToken cancellationToken = default);
        Task MarkAsReadAsync(int conversationId, int userId, CancellationToken cancellationToken = default);
    }
}
