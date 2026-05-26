using NhaXinh.Models;

namespace NhaXinh.Services.Interfaces
{
    public interface INewsService
    {
        Task<News?> GetPublishedBySlugAsync(string slug);

        Task<(List<News> Items, int TotalCount, int TotalPages)> GetPublishedPagedAsync(
            int page,
            int pageSize,
            string? keyword = null
        );

        Task<List<News>> GetFeaturedAsync(int count = 3);

        Task<List<News>> GetRelatedAsync(int newsId, int count = 3);

        Task<News?> GetByIdAsync(int id);

        Task<(List<News> Items, int TotalCount, int TotalPages)> GetAllPagedAsync(
            int page,
            int pageSize,
            string? keyword = null
        );

        Task<(bool Success, string Message)> CreateAsync(News news);
        Task<(bool Success, string Message)> UpdateAsync(News news);
        Task<(bool Success, string Message)> DeleteAsync(int id);

        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    }
}