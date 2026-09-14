namespace MusicApi.Response
{
    public class SongResponse
    {
        public int Id { get; set; }
        public string ? Title { get; set; }
        public string ? Artist { get; set; }
        public string ? Source { get; set; }
        public string ? Image { get; set; }
        public int Duration { get; set; }
        public int Counter { get; set; }
        public int Replay { get; set; }
        public bool Favorite { get; set; }
        public int? AlbumId { get; set; }
    }
}
