namespace MusicApi.Models
{
    public class Album : BaseEntity
    {
        public string ? Title { get; set; } 
        public string ? Artist { get; set; }
        public string ? Image { get; set; }  
        public ICollection<Song> ? Songs { get; set; }
    }
}
