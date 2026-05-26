namespace NhaXinh.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;     
        public string? ShortDescription { get; set; }         
        public string? Description { get; set; }            
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }            
        public int StockQuantity { get; set; } = 0;
        public int CategoryId { get; set; }
        public string? MainImageUrl { get; set; }            
        public bool IsFeatured { get; set; } = false;         
        public bool IsActive { get; set; } = true;
        public int ViewCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Category Category { get; set; } = null!;
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}