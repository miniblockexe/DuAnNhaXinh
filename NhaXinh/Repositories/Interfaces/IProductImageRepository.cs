using NhaXinh.Models;

namespace NhaXinh.Repositories.Interfaces
{
    public interface IProductImageRepository
    {
        Task<List<ProductImage>> GetByProductIdAsync(int productId);
        Task<ProductImage?> GetByIdAsync(int id);

        Task AddAsync(ProductImage image);
        Task AddRangeAsync(List<ProductImage> images);
        Task DeleteAsync(int id);
        Task DeleteByProductIdAsync(int productId);
    }
}