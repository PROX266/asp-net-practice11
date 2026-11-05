namespace WebApplication1.DTOs
{
    public class ProductReservationDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public ProductShortDto Product { get; set; } = null!;
        public UserShortDto User { get; set; } = null!;
    }
}
