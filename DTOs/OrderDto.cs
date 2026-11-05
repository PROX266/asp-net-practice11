namespace WebApplication1.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserShortDto User { get; set; } = null!;
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
