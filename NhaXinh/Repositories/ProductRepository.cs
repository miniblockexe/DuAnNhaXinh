using Microsoft.EntityFrameworkCore;
using NhaXinh.Data;
using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;

namespace NhaXinh.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetBySlugAsync(string slug)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);
        }

        public async Task<Product?> GetWithImagesAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages.OrderBy(img => img.DisplayOrder))
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            int? categoryId = null, string? keyword = null,
            decimal? minPrice = null, decimal? maxPrice = null,
            string? sortBy = null)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                var relatedIds = await _context.Categories
                    .Where(c => c.Id == categoryId || c.ParentId == categoryId)
                    .Select(c => c.Id)
                    .ToListAsync();

                query = query.Where(p => relatedIds.Contains(p.CategoryId));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(kw) ||
                    (p.ShortDescription != null && p.ShortDescription.ToLower().Contains(kw)));
            }

            if (minPrice.HasValue) query = query.Where(p => (p.DiscountPrice ?? p.Price) >= minPrice);
            if (maxPrice.HasValue) query = query.Where(p => (p.DiscountPrice ?? p.Price) <= maxPrice);

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.DiscountPrice ?? p.Price),
                "price_desc" => query.OrderByDescending(p => p.DiscountPrice ?? p.Price),
                "popular" => query.OrderByDescending(p => p.ViewCount),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<Product>> GetFeaturedAsync(int count = 6)
        {
            return await _context.Products
                .Where(p => p.IsFeatured && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Product>> GetRelatedAsync(int productId, int categoryId, int count = 4)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.Id != productId && p.IsActive)
                .OrderByDescending(p => p.ViewCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Product>> GetLowStockAsync(int threshold = 5)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity <= threshold && p.IsActive)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.Now;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return;

            product.IsActive = false;
            product.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return;

            product.StockQuantity -= quantity;
            product.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return;

            product.ViewCount++;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            return await _context.Products
                .AnyAsync(p => p.Slug == slug && (!excludeId.HasValue || p.Id != excludeId.Value));
        }
    }
}
