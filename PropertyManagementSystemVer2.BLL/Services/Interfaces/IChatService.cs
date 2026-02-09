using PropertyManagementSystemVer2.BLL.DTOs;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IChatService
    {
        Task<ServiceResultDto<ConversationDto>> CreateOrGetConversationAsync(int userId, CreateConversationDto dto);
        Task<ServiceResultDto<List<ConversationDto>>> GetConversationsAsync(int userId);
        Task<ServiceResultDto<List<MessageDto>>> GetMessagesAsync(int userId, int conversationId, int page = 1, int pageSize = 50);
        Task<ServiceResultDto<MessageDto>> SendMessageAsync(int userId, SendMessageDto dto);
        Task<ServiceResultDto> MarkAsReadAsync(int userId, int conversationId);
        Task<ServiceResultDto> DeleteMessageAsync(int userId, int messageId, bool deleteForAll);
        Task<ServiceResultDto<List<MessageDto>>> SearchMessagesAsync(int userId, int conversationId, string keyword);
    }
}
