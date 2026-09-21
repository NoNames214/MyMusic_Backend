using System.ComponentModel.DataAnnotations;

namespace MusicApi.Request
{
    public class ChangePasswordRequest
    {
        [Required]
        public string ? CurrentPassword { get; set; }
        [Required]
        public string ? NewPassword { get; set; }
    }
}
