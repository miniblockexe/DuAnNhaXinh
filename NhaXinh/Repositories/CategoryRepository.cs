using Microsoft.EntityFrameworkCore;
using NhaXinh.Data;
using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;

namespace NhaXinh.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
            => await _context.Categories
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();

        public async Task<List<Category>> GetAllActiveAsync()
            => await _context.Categories
                .Where(c => c.IsActive)
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();

        public async Task<List<Category>> GetParentCategoriesAsync()
            => await _context.Categories
                .Where(c => c.ParentId == null && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

        public async Task<List<Category>> GetParentCategoriesWithChildrenAsync()
            => await _context.Categories
                .Where(c => c.ParentId == null && c.IsActive)
                .Include(c => c.SubCategories.Where(s => s.IsActive))
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

        public async Task<List<Category>> GetSubCategoriesAsync(int parentId)
            => await _context.Categories
                .Where(c => c.ParentId == parentId && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

        public async Task<Category?> GetByIdAsync(int id)
            => await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Category?> GetBySlugAsync(string slug)
            => await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);

        public async Task<Category?> GetWithChildrenAsync(int id)
            => await _context.Categories
                .Include(c => c.SubCategories.Where(s => s.IsActive))
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            var tracked = _context.Categories.Local
                .FirstOrDefault(c => c.Id == category.Id);

            if (tracked is not null)
                _context.Entry(tracked).State = EntityState.Detached;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null) return;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasProductsAsync(int id)
            => await _context.Products.AnyAsync(p => p.CategoryId == id);

        public async Task<bool> HasSubCategoriesAsync(int id)
            => await _context.Categories.AnyAsync(c => c.ParentId == id);

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
            => await _context.Categories
                .AnyAsync(c => c.Slug == slug &&
                               (!excludeId.HasValue || c.Id != excludeId.Value));
    }
}