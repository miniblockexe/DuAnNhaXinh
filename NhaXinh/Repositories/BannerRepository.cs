using Microsoft.EntityFrameworkCore;
using NhaXinh.Data;
using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;

namespace NhaXinh.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly ApplicationDbContext _context;

        public BannerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Banner>> GetAllAsync()
        {
            return await _context.Banners
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<Banner>> GetActiveAsync()
        {
            return await _context.Banners
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Banner?> GetByIdAsync(int id)
        {
            return await _context.Banners.FindAsync(id);
        }

        public async Task AddAsync(Banner banner)
        {
            await _context.Banners.AddAsync(banner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Banner banner)
        {
            _context.Banners.Update(banner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) return;

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
        }
    }
}
