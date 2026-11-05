namespace WebApplication1.DTOs
{
    public class ProductShortDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
