using Microsoft.EntityFrameworkCore;
using NhaXinh.Data;
using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;

namespace NhaXinh.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductImage>> GetByProductIdAsync(int productId)
            => await _context.ProductImages
                .Where(img => img.ProductId == productId)
                .OrderBy(img => img.DisplayOrder)
                .ToListAsync();

        public async Task<ProductImage?> GetByIdAsync(int id)
            => await _context.ProductImages.FindAsync(id);

        public async Task AddAsync(ProductImage image)
        {
            await _context.ProductImages.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<ProductImage> images)
        {
            await _context.ProductImages.AddRangeAsync(images);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var image = await _context.ProductImages.FindAsync(id);
            if (image is null) return;

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByProductIdAsync(int productId)
        {
            var images = await _context.ProductImages
                .Where(img => img.ProductId == productId)
                .ToListAsync();

            if (images.Count == 0) return;

            _context.ProductImages.RemoveRange(images);
            await _context.SaveChangesAsync();
        }
    }
}