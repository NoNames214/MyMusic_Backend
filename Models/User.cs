using Microsoft.EntityFrameworkCore;

namespace MusicApi.Models
{
   public class User : BaseEntity
   {
      public string ? UserName { get; set; } 
      public string ? PasswordHash { get; set; } 
      public string? Email { get; set; }
      public string ? Avatar { get; set; }
      public string ? Role { get; set; }
      public bool isLocked { get; set; } = false;
      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      public ICollection<UserDevice> ? Devices { get; set; } 
      public ICollection<Playlist> ? Playlits { get; set; }
      public ICollection<Favorite> ? Favorites { get; set; }
      public ICollection<UserNotifications> ? UserNotifications { get; set; }
      public ICollection<ListeningHistory> ? listeningHistories { get; set; }
    }
}
