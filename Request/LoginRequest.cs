using System.ComponentModel.DataAnnotations;

namespace MusicApi.Request
{
    public class LoginRequest
    {
        [Required]
        public string? UserName { get; set; }
        [Required]  
        public string? PassWord { get; set; }
    }
}
