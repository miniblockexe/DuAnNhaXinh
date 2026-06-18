namespace NhaXinh.ViewModels.Home
{
    using NhaXinh.Models;
    public class HomeViewModel
    {
        public List<Banner> Banners { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }
}
