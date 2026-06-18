namespace NhaXinh.ViewModels.Product
{
    using NhaXinh.Models;
    public class ProductDetailViewModel
    {
        public Product Product { get; set; } = null!;
        public List<Product> RelatedProducts { get; set; } = new();
    }
}
