namespace MusicApi.Models
{
    public class Notification : BaseEntity
    {
        public string ? Title { get; set; } 
        public string ? Body { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<UserNotifications> ? UserNotifications { get; set; }
    }
}
