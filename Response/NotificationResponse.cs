namespace MusicApi.Response
{
    public class NotificationResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool isRead { get; set; }
    }
}
