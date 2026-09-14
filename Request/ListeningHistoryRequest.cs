using System.ComponentModel.DataAnnotations;

namespace MusicApi.Request
{
    public class ListeningHistoryRequest
    {
        [Required]
        public int SongId { get; set; }
    }
}
