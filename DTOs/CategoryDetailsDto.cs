namespace WebApplication1.DTOs
{
    public class CategoryDetailsDto
    {
        public int Id { get; set; }
        public string NameCategories { get; set; } = null!;
        public List<ProductShortDto> Products { get; set; } = new();
    }
}
