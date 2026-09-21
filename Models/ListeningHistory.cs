namespace MusicApi.Models
{
    public class ListeningHistory : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public int SongId { get; set; }
        public Song? Song { get; set; }
        public DateTimeOffset PlayedAt { get; set; }
    }
}
