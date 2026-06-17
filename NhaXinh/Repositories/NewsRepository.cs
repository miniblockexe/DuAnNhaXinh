using Microsoft.EntityFrameworkCore;
using NhaXinh.Data;
using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;

namespace NhaXinh.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly ApplicationDbContext _context;

        public NewsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<News?> GetByIdAsync(int id)
            => await _context.News.FindAsync(id);

        public async Task<News?> GetBySlugAsync(string slug)
            => await _context.News
                .FirstOrDefaultAsync(n => n.Slug == slug && n.IsPublished);

        public async Task<(List<News> Items, int TotalCount)> GetPublishedPagedAsync(
            int page, int pageSize, string? keyword = null)
        {
            var query = _context.News
                .Where(n => n.IsPublished)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(n =>
                    n.Title.ToLower().Contains(kw) ||
                    (n.Summary != null && n.Summary.ToLower().Contains(kw)));
            }

            query = query.OrderByDescending(n => n.PublishedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<News> Items, int TotalCount)> GetAllPagedAsync(
            int page, int pageSize, string? keyword = null)
        {
            var query = _context.News.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(kw));
            }

            query = query.OrderByDescending(n => n.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<News>> GetFeaturedAsync(int count = 3)
            => await _context.News
                .Where(n => n.IsFeatured && n.IsPublished)
                .OrderByDescending(n => n.PublishedAt)
                .Take(count)
                .ToListAsync();

        public async Task<List<News>> GetRelatedAsync(int excludeNewsId, int count = 3)
            => await _context.News
                .Where(n => n.Id != excludeNewsId && n.IsPublished)
                .OrderByDescending(n => n.PublishedAt)
                .Take(count)
                .ToListAsync();

        public async Task AddAsync(News news)
        {
            await _context.News.AddAsync(news);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(News news)
        {
            var tracked = _context.News.Local
                .FirstOrDefault(n => n.Id == news.Id);

            if (tracked is not null)
                _context.Entry(tracked).State = EntityState.Detached;

            _context.News.Update(news);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news is null) return;

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
            => await _context.News
                .AnyAsync(n => n.Slug == slug &&
                               (!excludeId.HasValue || n.Id != excludeId.Value));
    }
}