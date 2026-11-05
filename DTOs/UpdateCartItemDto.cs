namespace WebApplication1.DTOs
{
    public class UpdateCartItemDto
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
