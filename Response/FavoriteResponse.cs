namespace MusicApi.Response
{
    public class FavoriteResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SongId { get; set; }
        public string ? Artist { get; set; }
        public string ? Title { get; set; }
        public string ? Image { get; set; }
        public string ? UserName { get; set; }
        public int Counter { get; set; }
        public DateTime LikeAt { get; set; }
    }
}
