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
            => await _context.Banners
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();

        public async Task<List<Banner>> GetActiveAsync()
            => await _context.Banners
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();

        public async Task<Banner?> GetByIdAsync(int id)
            => await _context.Banners.FindAsync(id);

        public async Task AddAsync(Banner banner)
        {
            await _context.Banners.AddAsync(banner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Banner banner)
        {
            var tracked = _context.Banners.Local
                .FirstOrDefault(b => b.Id == banner.Id);

            if (tracked is not null)
                _context.Entry(tracked).State = EntityState.Detached;

            _context.Banners.Update(banner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<Banner> banners)
        {
            _context.Banners.UpdateRange(banners);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner is null) return;

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
        }
    }
}