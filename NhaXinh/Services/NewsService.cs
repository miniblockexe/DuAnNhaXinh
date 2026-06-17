using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly ILogger<NewsService> _logger;

        public NewsService(INewsRepository newsRepository, ILogger<NewsService> logger)
        {
            _newsRepository = newsRepository;
            _logger = logger;
        }

        public async Task<News?> GetPublishedBySlugAsync(string slug)
        {
            var news = await _newsRepository.GetBySlugAsync(slug);
            return news?.IsPublished == true ? news : null;
        }

        public async Task<(List<News> Items, int TotalCount, int TotalPages)> GetPublishedPagedAsync(
            int page, int pageSize, string? keyword = null)
        {
            (page, pageSize) = NormalizePaging(page, pageSize, defaultPageSize: 9);

            var (items, totalCount) = await _newsRepository.GetPublishedPagedAsync(page, pageSize, keyword);

            return (items, totalCount, CalcTotalPages(totalCount, pageSize));
        }

        public async Task<List<News>> GetFeaturedAsync(int count = 3)
            => await _newsRepository.GetFeaturedAsync(count);

        public async Task<List<News>> GetRelatedAsync(int newsId, int count = 3)
            => await _newsRepository.GetRelatedAsync(newsId, count);

        public async Task<News?> GetByIdAsync(int id)
            => await _newsRepository.GetByIdAsync(id);

        public async Task<(List<News> Items, int TotalCount, int TotalPages)> GetAllPagedAsync(
            int page, int pageSize, string? keyword = null)
        {
            (page, pageSize) = NormalizePaging(page, pageSize, defaultPageSize: 10);

            var (items, totalCount) = await _newsRepository.GetAllPagedAsync(page, pageSize, keyword);

            return (items, totalCount, CalcTotalPages(totalCount, pageSize));
        }

        public async Task<(bool Success, string Message)> CreateAsync(News news)
        {
            if (string.IsNullOrWhiteSpace(news.Title))
                return (false, "Tiêu đề bài viết không được để trống.");

            if (string.IsNullOrWhiteSpace(news.Slug))
                return (false, "Slug không được để trống.");

            if (string.IsNullOrWhiteSpace(news.Content))
                return (false, "Nội dung bài viết không được để trống.");

            if (await _newsRepository.SlugExistsAsync(news.Slug))
                return (false, $"Slug '{news.Slug}' đã tồn tại. Vui lòng chọn slug khác.");

            news.CreatedAt = DateTime.Now;

            if (news.IsPublished && news.PublishedAt is null)
                news.PublishedAt = DateTime.Now;

            await _newsRepository.AddAsync(news);
            return (true, "Thêm bài viết thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(News news)
        {
            var existing = await _newsRepository.GetByIdAsync(news.Id);
            if (existing is null)
                return (false, "Bài viết không tồn tại.");

            if (string.IsNullOrWhiteSpace(news.Title))
                return (false, "Tiêu đề bài viết không được để trống.");

            if (string.IsNullOrWhiteSpace(news.Slug))
                return (false, "Slug không được để trống.");

            if (string.IsNullOrWhiteSpace(news.Content))
                return (false, "Nội dung bài viết không được để trống.");

            if (await _newsRepository.SlugExistsAsync(news.Slug, news.Id))
                return (false, $"Slug '{news.Slug}' đã tồn tại. Vui lòng chọn slug khác.");

            news.PublishedAt = news.IsPublished
                ? (existing.PublishedAt ?? DateTime.Now)
                : null;

            await _newsRepository.UpdateAsync(news);
            return (true, "Cập nhật bài viết thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var existing = await _newsRepository.GetByIdAsync(id);
            if (existing is null)
                return (false, "Bài viết không tồn tại.");

            await _newsRepository.DeleteAsync(id);
            return (true, "Xoá bài viết thành công.");
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
            => await _newsRepository.SlugExistsAsync(slug, excludeId);


        public async Task<(List<News> Items, int TotalCount)> GetPublishedNewsAsync(
            int page, int pageSize, string? keyword = null)
        {
            var (items, totalCount, _) = await GetPublishedPagedAsync(page, pageSize, keyword);
            return (items, totalCount);
        }

        public async Task<List<News>> GetFeaturedNewsAsync(int count = 5)
            => await GetFeaturedAsync(count);

        public async Task<News?> GetBySlugAsync(string slug)
            => await GetPublishedBySlugAsync(slug);

        public async Task<List<News>> GetRelatedNewsAsync(int excludeId, int count = 4)
            => await GetRelatedAsync(excludeId, count);


        private static (int page, int pageSize) NormalizePaging(int page, int pageSize, int defaultPageSize)
            => (Math.Max(1, page), pageSize < 1 ? defaultPageSize : pageSize);

        private static int CalcTotalPages(int totalCount, int pageSize)
            => (int)Math.Ceiling((double)totalCount / pageSize);
    }
}