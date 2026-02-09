using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        // In-memory notifications store (sẽ thay bằng DB entity sau)
        private static readonly List<NotificationDto> _notifications = new();
        private static int _nextId = 1;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR51: Thông báo trong ứng dụng (In-app Notification)
        // 1. Real-time notification cho tất cả events quan trọng
        // 2. Bell icon với unread count
        // 3. Mark as read (individual/all)
        public async Task SendNotificationAsync(int userId, string title, string message, string? link = null)
        {
            var notification = new NotificationDto
            {
                Id = _nextId++,
                UserId = userId,
                Title = title,
                Message = message,
                Link = link,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _notifications.Add(notification);

            // BR52: TODO - Gửi email notification nếu user bật
            // BR53: TODO - Gửi push notification nếu user opt-in
            await Task.CompletedTask;
        }

        public async Task<ServiceResultDto<List<NotificationDto>>> GetNotificationsAsync(int userId)
        {
            var userNotifications = _notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return await Task.FromResult(ServiceResultDto<List<NotificationDto>>.Success(userNotifications));
        }

        // BR51.2: Unread count
        public async Task<ServiceResultDto<int>> GetUnreadCountAsync(int userId)
        {
            var count = _notifications.Count(n => n.UserId == userId && !n.IsRead);
            return await Task.FromResult(ServiceResultDto<int>.Success(count));
        }

        // BR51.3: Mark as read
        public async Task<ServiceResultDto> MarkAsReadAsync(int userId, int notificationId)
        {
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId && n.UserId == userId);
            if (notification != null) notification.IsRead = true;
            return await Task.FromResult(ServiceResultDto.Success("Đã đánh dấu đã đọc."));
        }

        // BR51.3: Mark all as read
        public async Task<ServiceResultDto> MarkAllAsReadAsync(int userId)
        {
            foreach (var n in _notifications.Where(n => n.UserId == userId && !n.IsRead))
            {
                n.IsRead = true;
            }
            return await Task.FromResult(ServiceResultDto.Success("Đã đánh dấu tất cả đã đọc."));
        }
    }
}
