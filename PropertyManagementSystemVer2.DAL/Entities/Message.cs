using PropertyManagementSystemVer2.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class Message
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public int SenderId { get; set; }

        public MessageType MessageType { get; set; } = MessageType.Text;

        // Nội dung tin nhắn
        public string Content { get; set; } = string.Empty;

        // Attachments (file, image, document)
        public string? AttachmentUrl { get; set; }

        public string? AttachmentName { get; set; }

        public string? AttachmentMimeType { get; set; }

        public long? AttachmentSize { get; set; } // bytes

        // Metadata cho tin nhắn đặc biệt
        public string? Metadata { get; set; } // JSON: {propertyId: 1, action: "schedule_viewing"}

        // Trạng thái
        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }

        public bool IsEdited { get; set; } = false;

        public DateTime? EditedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        // Reply to another message
        public int? ReplyToMessageId { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Conversation Conversation { get; set; } = null!;

        public User Sender { get; set; } = null!;

        public Message? ReplyToMessage { get; set; }

        public ICollection<Message> Replies { get; set; } = new List<Message>();
    }
}
