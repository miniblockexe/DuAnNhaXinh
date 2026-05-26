using NhaXinh.Models;

namespace NhaXinh.Repositories.Interfaces
{
    public interface INewsRepository
    {
        Task<News?> GetByIdAsync(int id);
        Task<News?> GetBySlugAsync(string slug);

        Task<(List<News> Items, int TotalCount)> GetPublishedPagedAsync(
            int page,
            int pageSize,
            string? keyword = null
        );

        Task<(List<News> Items, int TotalCount)> GetAllPagedAsync(
            int page,
            int pageSize,
            string? keyword = null
        );

        Task<List<News>> GetFeaturedAsync(int count = 3);
        Task<List<News>> GetRelatedAsync(int excludeNewsId, int count = 3);

        Task AddAsync(News news);
        Task UpdateAsync(News news);
        Task DeleteAsync(int id);

        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    }
}
