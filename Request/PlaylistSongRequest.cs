using System.ComponentModel.DataAnnotations;

namespace MusicApi.Request
{
    public class PlaylistSongRequest
    {
        [Required]
        public int PlaylistId { get; set; }
        [Required]
        public int SongId { get; set; }
    }
}
