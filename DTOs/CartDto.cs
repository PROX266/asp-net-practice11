namespace WebApplication1.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        public UserShortDto User { get; set; } = null!;
        public List<CartItemDto> Items { get; set; } = new();
    }
}
