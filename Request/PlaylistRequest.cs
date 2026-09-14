using System.ComponentModel.DataAnnotations;

namespace MusicApi.Request
{
    public class PlaylistRequest
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
