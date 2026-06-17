using NhaXinh.Models;

namespace NhaXinh.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<List<Category>> GetAllActiveAsync();
        Task<List<Category>> GetParentCategoriesAsync();
        Task<List<Category>> GetParentCategoriesWithChildrenAsync();
        Task<List<Category>> GetSubCategoriesAsync(int parentId);

        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetBySlugAsync(string slug);
        Task<Category?> GetWithChildrenAsync(int id);

        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task<bool> HasProductsAsync(int id);
        Task<bool> HasSubCategoriesAsync(int id);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    }
}
