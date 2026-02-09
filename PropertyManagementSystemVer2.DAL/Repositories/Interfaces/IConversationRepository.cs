using PropertyManagementSystemVer2.DAL.Entities;

namespace PropertyManagementSystemVer2.DAL.Repositories.Interfaces
{
    public interface IConversationRepository : IGenericRepository<Conversation>
    {
        Task<Conversation?> GetByIdWithMessagesAsync(int id, int messageCount = 50, CancellationToken cancellationToken = default);
        Task<IEnumerable<Conversation>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Conversation?> GetByUsersAndContextAsync(int user1Id, int user2Id, int? propertyId = null, int? leaseId = null, int? bookingId = null, CancellationToken cancellationToken = default);
    }
}
