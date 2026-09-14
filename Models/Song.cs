namespace MusicApi.Models
{
    public class Song : BaseEntity
    {
        public string ?Title { get; set; }
        public string ?Artist { get; set; }
        public string ?Source { get; set; }
        public string ?Image { get; set; }
        public int Duration { get; set; }
        public int Counter { get; set; }
        public int Replay { get; set; }
        public int ? AlbumId { get; set; }
        public Album ? Album { get; set; }
        public ICollection<PlaylistSong> ? PlaylistSongs { get; set; }
        public ICollection<Favorite> ? Favorites { get; set; }
        public ICollection<ListeningHistory> ? listeningHistories { get; set; }
    }
}
