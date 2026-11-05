namespace WebApplication1.DTOs
{
    public class CreateUserDto
    {
        public string UserLogin { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
    }
}
