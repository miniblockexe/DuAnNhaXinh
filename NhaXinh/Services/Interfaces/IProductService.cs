using NhaXinh.Models;

namespace NhaXinh.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetBySlugAsync(string slug);
        Task<Product?> GetWithImagesAsync(int id);

        Task<(List<Product> Items, int TotalCount, int TotalPages)> GetPagedAsync(
            int page,
            int pageSize,
            int? categoryId = null,
            string? keyword = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = null
        );

        Task<List<Product>> GetFeaturedAsync(int count = 6);

        Task<List<Product>> GetRelatedAsync(int productId, int categoryId, int count = 4);

        Task<List<Product>> GetLowStockAsync(int threshold = 5);

        Task<(bool Success, string Message)> CreateAsync(Product product);
        Task<(bool Success, string Message)> UpdateAsync(Product product);
        Task<(bool Success, string Message)> DeleteAsync(int id);

        Task IncrementViewCountAsync(int id);

        Task<bool> IsInStockAsync(int productId, int requiredQuantity = 1);

        Task<(bool Success, string Message)> ReduceStockAsync(int productId, int quantity);

        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    }
}
