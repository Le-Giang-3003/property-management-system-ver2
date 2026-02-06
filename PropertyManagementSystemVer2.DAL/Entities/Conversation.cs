using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class Conversation
    {
        public int Id { get; set; }

        // Participants (2 users)
        public int User1Id { get; set; }

        public int User2Id { get; set; }

        // Context của cuộc trò chuyện (optional)
        public int? PropertyId { get; set; } // Đang thảo luận về property nào

        public int? LeaseId { get; set; } // Đang thảo luận về lease nào

        public int? BookingId { get; set; } // Đang thảo luận về booking nào

        // Thông tin cuộc trò chuyện
        public string? Title { get; set; } // Tiêu đề tự động: "Chat về [Property Title]"

        public DateTime LastMessageAt { get; set; } // Tin nhắn cuối cùng

        public int LastMessageId { get; set; } // ID của tin nhắn cuối

        // Trạng thái
        public bool IsActive { get; set; } = true;

        public bool IsArchived { get; set; } = false;

        // Đếm tin nhắn chưa đọc cho mỗi user
        public int UnreadCountUser1 { get; set; } = 0;

        public int UnreadCountUser2 { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public User User1 { get; set; } = null!;

        public User User2 { get; set; } = null!;

        public Property? Property { get; set; }

        public Lease? Lease { get; set; }

        public Booking? Booking { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
