namespace MusicApi.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool isDeleted { get; set; } = false;
        public DateTime ? DeleteAt { get; set; } 
        public string ? DeletedBy { get; set; }
    }
}
