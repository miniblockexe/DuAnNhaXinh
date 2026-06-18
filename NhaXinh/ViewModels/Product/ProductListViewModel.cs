using NhaXinh.Models;

namespace NhaXinh.ViewModels.Product
{
    public class ProductListViewModel
    {
        public List<NhaXinh.Models.Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; } = 0;

        public int? CategoryId { get; set; }
        public string? Keyword { get; set; }
        public string? SortBy { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
