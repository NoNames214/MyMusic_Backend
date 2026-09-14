using System.ComponentModel.DataAnnotations;

namespace MusicApi.Request
{
    public class RegisterRequest
    {
        [Required]
        public string ? UserName { get; set; }
        [Required]
        public string ? PassWord { get; set; }
    }
}
    