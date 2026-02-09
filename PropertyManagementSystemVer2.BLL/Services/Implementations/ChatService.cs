using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR46: Tạo Conversation
        // 1. Auto-create khi: Tenant apply property, Booking được tạo, Lease active
        // 2. Context-aware: gắn với Property/Lease/Booking
        // 3. 1 cặp user chỉ có 1 conversation/context
        public async Task<ServiceResultDto<ConversationDto>> CreateOrGetConversationAsync(int userId, CreateConversationDto dto)
        {
            // BR46.3: Kiểm tra đã có conversation chưa
            var existing = await _unitOfWork.Conversations.GetByUsersAndContextAsync(
                userId, dto.OtherUserId, dto.PropertyId, dto.LeaseId, dto.BookingId);

            if (existing != null)
            {
                return ServiceResultDto<ConversationDto>.Success(MapToConversationDto(existing, userId));
            }

            // Tạo conversation mới
            var otherUser = await _unitOfWork.Users.GetByIdAsync(dto.OtherUserId);
            if (otherUser == null)
                return ServiceResultDto<ConversationDto>.Failure("Không tìm thấy người dùng.");

            string? title = null;
            if (dto.PropertyId.HasValue)
            {
                var property = await _unitOfWork.Properties.GetByIdAsync(dto.PropertyId.Value);
                title = $"Chat về {property?.Title ?? "property"}";
            }

            var conversation = new Conversation
            {
                User1Id = userId,
                User2Id = dto.OtherUserId,
                PropertyId = dto.PropertyId,
                LeaseId = dto.LeaseId,
                BookingId = dto.BookingId,
                Title = title,
                LastMessageAt = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Conversations.AddAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            // Reload with navigation
            var result = await _unitOfWork.Conversations.GetByIdWithMessagesAsync(conversation.Id);
            return ServiceResultDto<ConversationDto>.Success(MapToConversationDto(result!, userId));
        }

        // BR50: Danh sách Conversation
        // 1. Load tin nhắn theo pagination (50 messages/batch)
        public async Task<ServiceResultDto<List<ConversationDto>>> GetConversationsAsync(int userId)
        {
            var conversations = await _unitOfWork.Conversations.GetByUserIdAsync(userId);
            return ServiceResultDto<List<ConversationDto>>.Success(
                conversations.Select(c => MapToConversationDto(c, userId)).ToList());
        }

        // BR50.1: Load messages với pagination
        public async Task<ServiceResultDto<List<MessageDto>>> GetMessagesAsync(int userId, int conversationId, int page = 1, int pageSize = 50)
        {
            var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
            if (conversation == null || (conversation.User1Id != userId && conversation.User2Id != userId))
                return ServiceResultDto<List<MessageDto>>.Failure("Không có quyền xem cuộc trò chuyện này.");

            var messages = await _unitOfWork.Messages.GetByConversationIdAsync(conversationId, page, pageSize);
            return ServiceResultDto<List<MessageDto>>.Success(messages.Select(MapToMessageDto).ToList());
        }

        // BR47: Gửi/Nhận tin nhắn
        // 1. Real-time qua SignalR (xử lý ở tầng Web)
        // 2. Hỗ trợ text + file attachment (image, PDF, max 10MB)
        // 3. Message status: Sent/Delivered/Read
        // 4. Không gửi tin cho user đã bị block
        public async Task<ServiceResultDto<MessageDto>> SendMessageAsync(int userId, SendMessageDto dto)
        {
            var conversation = await _unitOfWork.Conversations.GetByIdAsync(dto.ConversationId);
            if (conversation == null)
                return ServiceResultDto<MessageDto>.Failure("Không tìm thấy cuộc trò chuyện.");

            if (conversation.User1Id != userId && conversation.User2Id != userId)
                return ServiceResultDto<MessageDto>.Failure("Bạn không thuộc cuộc trò chuyện này.");

            if (!conversation.IsActive)
                return ServiceResultDto<MessageDto>.Failure("Cuộc trò chuyện đã bị khóa.");

            var message = new Message
            {
                ConversationId = dto.ConversationId,
                SenderId = userId,
                MessageType = dto.MessageType,
                Content = dto.Content,
                AttachmentUrl = dto.AttachmentUrl,
                AttachmentName = dto.AttachmentName,
                AttachmentMimeType = dto.AttachmentMimeType,
                AttachmentSize = dto.AttachmentSize,
                ReplyToMessageId = dto.ReplyToMessageId,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Messages.AddAsync(message);

            // Cập nhật conversation
            conversation.LastMessageAt = DateTime.UtcNow;
            conversation.LastMessageId = message.Id;

            // Tăng unread count cho người nhận
            if (conversation.User1Id == userId)
                conversation.UnreadCountUser2++;
            else
                conversation.UnreadCountUser1++;

            conversation.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Conversations.Update(conversation);

            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto<MessageDto>.Success(MapToMessageDto(message));
        }

        // BR47.3: Đánh dấu đã đọc
        public async Task<ServiceResultDto> MarkAsReadAsync(int userId, int conversationId)
        {
            var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
            if (conversation == null)
                return ServiceResultDto.Failure("Không tìm thấy cuộc trò chuyện.");

            // Reset unread count
            if (conversation.User1Id == userId)
                conversation.UnreadCountUser1 = 0;
            else if (conversation.User2Id == userId)
                conversation.UnreadCountUser2 = 0;

            _unitOfWork.Conversations.Update(conversation);

            // Mark messages as read
            await _unitOfWork.Messages.MarkAsReadAsync(conversationId, userId);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Đã đánh dấu đã đọc.");
        }

        // BR49: Xóa tin nhắn
        // 1. Soft delete, xóa phía mình hoặc xóa cho cả 2
        // 2. Xóa cho cả 2 chỉ trong 15 phút sau gửi
        // 3. Admin có thể xóa bất kỳ (moderation)
        public async Task<ServiceResultDto> DeleteMessageAsync(int userId, int messageId, bool deleteForAll)
        {
            var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
            if (message == null)
                return ServiceResultDto.Failure("Không tìm thấy tin nhắn.");

            if (message.SenderId != userId)
                return ServiceResultDto.Failure("Bạn chỉ có thể xóa tin nhắn của mình.");

            // BR49.2: Xóa cho cả 2 chỉ trong 15 phút
            if (deleteForAll)
            {
                var minutesSinceSent = (DateTime.UtcNow - message.SentAt).TotalMinutes;
                if (minutesSinceSent > 15)
                    return ServiceResultDto.Failure("Chỉ có thể xóa cho cả 2 trong vòng 15 phút sau gửi.");
            }

            // BR49.1: Soft delete
            message.IsDeleted = true;
            message.DeletedAt = DateTime.UtcNow;
            _unitOfWork.Messages.Update(message);

            // Lưu vào DeletedMessages
            var deletedMessage = new DeletedMessage
            {
                MessageId = message.Id,
                ConversationId = message.ConversationId,
                OriginalContent = message.Content,
                DeletedBy = userId,
                DeletedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<DeletedMessage>().AddAsync(deletedMessage);

            await _unitOfWork.SaveChangesAsync();
            return ServiceResultDto.Success("Đã xóa tin nhắn.");
        }

        // BR50.2: Tìm kiếm trong conversation
        public async Task<ServiceResultDto<List<MessageDto>>> SearchMessagesAsync(int userId, int conversationId, string keyword)
        {
            var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
            if (conversation == null || (conversation.User1Id != userId && conversation.User2Id != userId))
                return ServiceResultDto<List<MessageDto>>.Failure("Không có quyền tìm kiếm.");

            var messages = await _unitOfWork.Messages.SearchInConversationAsync(conversationId, keyword);
            return ServiceResultDto<List<MessageDto>>.Success(messages.Select(MapToMessageDto).ToList());
        }

        // Mapping helpers
        private static ConversationDto MapToConversationDto(Conversation c, int currentUserId)
        {
            var isUser1 = c.User1Id == currentUserId;
            var otherUser = isUser1 ? c.User2 : c.User1;

            return new ConversationDto
            {
                Id = c.Id,
                OtherUserId = otherUser?.Id ?? 0,
                OtherUserName = otherUser?.FullName ?? string.Empty,
                OtherUserAvatar = otherUser?.AvatarUrl,
                Title = c.Title,
                PropertyId = c.PropertyId,
                PropertyTitle = c.Property?.Title,
                LastMessageAt = c.LastMessageAt,
                LastMessageContent = c.Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Content,
                UnreadCount = isUser1 ? c.UnreadCountUser1 : c.UnreadCountUser2,
                IsActive = c.IsActive
            };
        }

        private static MessageDto MapToMessageDto(Message m)
        {
            return new MessageDto
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                SenderName = m.Sender?.FullName ?? string.Empty,
                SenderAvatar = m.Sender?.AvatarUrl,
                MessageType = m.MessageType,
                Content = m.Content,
                AttachmentUrl = m.AttachmentUrl,
                AttachmentName = m.AttachmentName,
                AttachmentSize = m.AttachmentSize,
                IsRead = m.IsRead,
                ReadAt = m.ReadAt,
                IsEdited = m.IsEdited,
                IsDeleted = m.IsDeleted,
                ReplyToMessageId = m.ReplyToMessageId,
                ReplyToContent = m.ReplyToMessage?.Content,
                SentAt = m.SentAt
            };
        }
    }
}
