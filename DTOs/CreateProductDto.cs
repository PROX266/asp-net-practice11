namespace WebApplication1.DTOs
{
    public class CreateProductDto
    {
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int StockQuantity { get; set; }
        public string? ImagePath { get; set; }
    }
}
