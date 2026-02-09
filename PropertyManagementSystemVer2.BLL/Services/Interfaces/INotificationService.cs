using PropertyManagementSystemVer2.BLL.DTOs;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int userId, string title, string message, string? link = null);
        Task<ServiceResultDto<List<NotificationDto>>> GetNotificationsAsync(int userId);
        Task<ServiceResultDto<int>> GetUnreadCountAsync(int userId);
        Task<ServiceResultDto> MarkAsReadAsync(int userId, int notificationId);
        Task<ServiceResultDto> MarkAllAsReadAsync(int userId);
    }
}
