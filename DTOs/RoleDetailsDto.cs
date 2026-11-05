namespace WebApplication1.DTOs
{
    public class RoleDetailsDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public List<UserShortDto> Users { get; set; } = new();
    }
}
