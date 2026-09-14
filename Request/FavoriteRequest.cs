using System.ComponentModel.DataAnnotations;

namespace MusicApi.Request
{
    public class FavoriteRequest
    {
        [Required]
        public int SongId { get; set; }
    }
}
