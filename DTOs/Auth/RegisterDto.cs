using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs.Auth
{
    public class RegisterDto
    {
        [Required, StringLength(50, MinimumLength = 3)]
        public string Login { get; set; } = "";

        [Required, MinLength(6)]
        public string Password { get; set; } = "";
    }
}
