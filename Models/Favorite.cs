namespace MusicApi.Models
{
    public class Favorite : BaseEntity
    {
        public int UserId { get; set; }
        public User ? User { get; set; }
        public int SongId { get; set; }
        public Song ? Song { get; set; }
        public DateTime ? LikeAt { get; set; } 
    }
}
