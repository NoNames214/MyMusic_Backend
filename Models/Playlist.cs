namespace MusicApi.Models
{
    public class Playlist : BaseEntity
    {
        public string ? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int UserId { get; set; }
        public User ? User { get; set; }
        public ICollection<PlaylistSong> ? PlaylistSongs { get; set; }
    }
}
