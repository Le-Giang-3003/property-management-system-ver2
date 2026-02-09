using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.BLL.DTOs
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public int OtherUserId { get; set; }
        public string OtherUserName { get; set; } = string.Empty;
        public string? OtherUserAvatar { get; set; }
        public string? Title { get; set; }
        public int? PropertyId { get; set; }
        public string? PropertyTitle { get; set; }
        public DateTime LastMessageAt { get; set; }
        public string? LastMessageContent { get; set; }
        public int UnreadCount { get; set; }
        public bool IsActive { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? SenderAvatar { get; set; }
        public MessageType MessageType { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
        public string? AttachmentName { get; set; }
        public long? AttachmentSize { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsEdited { get; set; }
        public bool IsDeleted { get; set; }
        public int? ReplyToMessageId { get; set; }
        public string? ReplyToContent { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class SendMessageDto
    {
        public int ConversationId { get; set; }
        public MessageType MessageType { get; set; } = MessageType.Text;
        public string Content { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
        public string? AttachmentName { get; set; }
        public string? AttachmentMimeType { get; set; }
        public long? AttachmentSize { get; set; }
        public int? ReplyToMessageId { get; set; }
    }

    public class CreateConversationDto
    {
        public int OtherUserId { get; set; }
        public int? PropertyId { get; set; }
        public int? LeaseId { get; set; }
        public int? BookingId { get; set; }
    }
}
