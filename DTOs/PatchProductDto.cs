namespace WebApplication1.DTOs
{
    public class PatchProductDto
    {
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }
        public int? CategoryId { get; set; }
        public int? StockQuantity { get; set; }
        public string? ImagePath { get; set; }
    }
}
