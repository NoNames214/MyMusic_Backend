namespace MusicApi.Models
{
    public class UserNotifications
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public int NotificationId { get; set; }
        public Notification? Notification { get; set; }
        public bool isRead { get; set; } 
        public DateTime readAt { get; set; } = DateTime.UtcNow;
    }
}
