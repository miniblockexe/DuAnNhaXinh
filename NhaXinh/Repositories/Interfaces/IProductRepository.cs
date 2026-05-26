using NhaXinh.Models;

namespace NhaXinh.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetBySlugAsync(string slug);
        Task<Product?> GetWithImagesAsync(int id);

        Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
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

        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);

        Task UpdateStockAsync(int productId, int quantity);
        Task IncrementViewCountAsync(int id);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    }
}
