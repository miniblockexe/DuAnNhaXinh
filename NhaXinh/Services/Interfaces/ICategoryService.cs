using NhaXinh.Models;

namespace NhaXinh.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<List<Category>> GetAllActiveAsync();

        Task<List<Category>> GetParentCategoriesWithChildrenAsync();
        Task<List<Category>> GetParentCategoriesAsync();
        Task<List<Category>> GetSubCategoriesAsync(int parentId);

        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetBySlugAsync(string slug);

        Task<(bool Success, string Message)> CreateAsync(Category category);
        Task<(bool Success, string Message)> UpdateAsync(Category category);

        Task<(bool Success, string Message)> DeleteAsync(int id);

        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    }
}