namespace NhaXinh.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }

        public decimal DisplayPrice => DiscountPrice ?? Price;
        public bool IsOnSale => DiscountPrice.HasValue && DiscountPrice < Price;

        public int StockQuantity { get; set; }
        public bool IsInStock => StockQuantity > 0;

        public string? MainImageUrl { get; set; }
        public bool IsFeatured { get; set; }
        public string? CategoryName { get; set; }
    }
}
