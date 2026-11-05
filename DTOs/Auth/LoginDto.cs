using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        public string Login { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
}
