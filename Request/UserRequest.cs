using System.ComponentModel.DataAnnotations;

namespace MusicApi.Request
{
    public class UserRequest
    {
        [Required]
        [MaxLength(50)]
        public string? UserName { get; set; }
        [Required]
        public string? Email { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
