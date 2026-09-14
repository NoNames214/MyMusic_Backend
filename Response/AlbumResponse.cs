namespace MusicApi.Response
{
    public class AlbumResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Artist { get; set; }
        public string? Image { get; set; }
        public List<SongResponse> ? Songs { get; set; }
    }
}
