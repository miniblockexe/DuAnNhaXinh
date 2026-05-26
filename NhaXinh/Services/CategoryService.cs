using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<List<Category>> GetAllActiveAsync()
        {
            return await _categoryRepository.GetAllActiveAsync();
        }

        public async Task<List<Category>> GetParentCategoriesWithChildrenAsync()
        {
            var parents = await _categoryRepository.GetParentCategoriesAsync();

            foreach (var parent in parents)
            {
                parent.SubCategories = await _categoryRepository.GetSubCategoriesAsync(parent.Id);
            }

            return parents;
        }

        public async Task<List<Category>> GetSubCategoriesAsync(int parentId)
        {
            return await _categoryRepository.GetSubCategoriesAsync(parentId);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category?> GetBySlugAsync(string slug)
        {
            return await _categoryRepository.GetBySlugAsync(slug);
        }

        public async Task<(bool Success, string Message)> CreateAsync(Category category)
        {
            if (await _categoryRepository.SlugExistsAsync(category.Slug))
                return (false, $"Slug '{category.Slug}' đã tồn tại. Vui lòng chọn slug khác.");

            if (category.ParentId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(category.ParentId.Value);
                if (parent == null)
                    return (false, "Danh mục cha không tồn tại.");
            }

            category.CreatedAt = DateTime.Now;
            await _categoryRepository.AddAsync(category);

            return (true, "Tạo danh mục thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Category category)
        {
            var existing = await _categoryRepository.GetByIdAsync(category.Id);
            if (existing == null)
                return (false, "Danh mục không tồn tại.");

            if (await _categoryRepository.SlugExistsAsync(category.Slug, category.Id))
                return (false, $"Slug '{category.Slug}' đã tồn tại. Vui lòng chọn slug khác.");

            if (category.ParentId.HasValue && category.ParentId.Value == category.Id)
                return (false, "Danh mục không thể là cha của chính nó.");

            await _categoryRepository.UpdateAsync(category);
            return (true, "Cập nhật danh mục thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return (false, "Danh mục không tồn tại.");

            if (await _categoryRepository.HasSubCategoriesAsync(id))
                return (false, "Không thể xóa danh mục đang có danh mục con.");

            if (await _categoryRepository.HasProductsAsync(id))
                return (false, "Không thể xóa danh mục đang chứa sản phẩm.");

            await _categoryRepository.DeleteAsync(id);
            return (true, "Xóa danh mục thành công.");
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            return await _categoryRepository.SlugExistsAsync(slug, excludeId);
        }
    }
}
