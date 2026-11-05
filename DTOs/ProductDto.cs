namespace WebApplication1.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImagePath { get; set; }
        public CategoryDto Category { get; set; } = null!;
    }
}
