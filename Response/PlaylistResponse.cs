namespace MusicApi.Response
{
    public class PlaylistResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string ? UserName { get; set; }
        public ICollection<SongResponse> ? Songs { get; set; }
    }
}
