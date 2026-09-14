namespace MusicApi.Models
{
    public class UserDevice : BaseEntity
    {
        public string? FcmToken { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int  UserId { get; set; }

        public User? User { get; set; }
    }
}
