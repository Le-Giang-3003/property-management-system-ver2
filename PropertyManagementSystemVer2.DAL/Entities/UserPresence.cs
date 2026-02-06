using PropertyManagementSystemVer2.DAL.Enums;

namespace PropertyManagementSystemVer2.DAL.Entities
{
    public class UserPresence
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public PresenceStatus Status { get; set; } = PresenceStatus.Offline;

        public string? ConnectionId { get; set; } // SignalR ConnectionId

        public DateTime? LastSeenAt { get; set; }

        public DateTime? LastActiveAt { get; set; }

        public string? DeviceInfo { get; set; } // Browser/Device info

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
