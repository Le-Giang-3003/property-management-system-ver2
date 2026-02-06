using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class TypingIndicator
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public int UserId { get; set; }

        public bool IsTyping { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? StoppedAt { get; set; }

        // Navigation properties
        public Conversation Conversation { get; set; } = null!;

        public User User { get; set; } = null!;
    }
}
