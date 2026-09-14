using System.ComponentModel.DataAnnotations;

namespace MusicApi.Request
{
    public class AlbumRequest
    {
        [Required]
        public string ? Title { get; set; }
        [Required]
        public string ? Artist { get; set; }
        [Required]
        public string ? Image { get; set; }
    }
}
