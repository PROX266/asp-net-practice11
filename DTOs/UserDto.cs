namespace WebApplication1.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserLogin { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public bool IsActive { get; set; }
        public RoleDto Role { get; set; } = null!;
    }
}
