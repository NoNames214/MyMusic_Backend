using MusicApi.Request;
using MusicApi.Response;

namespace MusicApi.IService
{
    public interface INotificationService
    {
        Task<bool> SendAll (NotificationRequest request);
        Task<bool> SendToUser(int userId, NotificationRequest request);
        Task<List<NotificationResponse>> GetNotifications(int userId);
        Task<bool> MarkAsRead(int userId, int notificationId);
        Task<bool> DeleteNotification(int userId, int notificationId);
        Task<bool> DeleteAll(int userId);
        Task<int> UnReadCount(int userId);
    }
}
