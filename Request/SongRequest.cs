using System.ComponentModel.DataAnnotations;

namespace MusicApi.Request
{
    public class SongRequest
    {
        [Required]
        public string ? Title { get; set; }
        [Required]
        public string ? Artist { get; set; }
        [Required]
        public string ? Source { get; set; }
        [Required]
        public string ? Image { get; set; }
        [Required]
        [Range(1, 10000)]
        public int Duration { get; set; }
        [Required]
        public int ? AlbumId { get; set; }
    }
}
