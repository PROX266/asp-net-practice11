namespace WebApplication1.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = "";
        public int ExpiresInMinutes { get; set; }
    }
}
