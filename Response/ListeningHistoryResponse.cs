namespace MusicApi.Response
{
    public class ListeningHistoryResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SongId { get; set; }
        public string? Artist { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public DateTime PlayedAt { get; set; }
    }
}
