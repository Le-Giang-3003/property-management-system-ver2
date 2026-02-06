using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class DeletedMessage
    {
        public int Id { get; set; }

        public int MessageId { get; set; }

        public int ConversationId { get; set; }

        public int SenderId { get; set; }

        public int DeletedBy { get; set; }

        public string OriginalContent { get; set; } = string.Empty;

        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;

        public string? Reason { get; set; }
    }
}
