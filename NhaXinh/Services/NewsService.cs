using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;

        public NewsService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<News?> GetPublishedBySlugAsync(string slug)
        {
            var news = await _newsRepository.GetBySlugAsync(slug);

            if (news == null || !news.IsPublished)
                return null;

            return news;
        }

        public async Task<(List<News> Items, int TotalCount, int TotalPages)> GetPublishedPagedAsync(
            int page,
            int pageSize,
            string? keyword = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 9;

            var (items, totalCount) = await _newsRepository.GetPublishedPagedAsync(page, pageSize, keyword);
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return (items, totalCount, totalPages);
        }

        public async Task<List<News>> GetFeaturedAsync(int count = 3)
        {
            return await _newsRepository.GetFeaturedAsync(count);
        }

        public async Task<List<News>> GetRelatedAsync(int newsId, int count = 3)
        {
            return await _newsRepository.GetRelatedAsync(newsId, count);
        }


        public async Task<News?> GetByIdAsync(int id)
        {
            return await _newsRepository.GetByIdAsync(id);
        }

        public async Task<(List<News> Items, int TotalCount, int TotalPages)> GetAllPagedAsync(
            int page,
            int pageSize,
            string? keyword = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, totalCount) = await _newsRepository.GetAllPagedAsync(page, pageSize, keyword);
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return (items, totalCount, totalPages);
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

            if (news.IsPublished && news.PublishedAt == null)
                news.PublishedAt = DateTime.Now;

            news.CreatedAt = DateTime.Now;

            await _newsRepository.AddAsync(news);
            return (true, "Thêm bài viết thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(News news)
        {
            var existing = await _newsRepository.GetByIdAsync(news.Id);
            if (existing == null)
                return (false, "Bài viết không tồn tại.");

            if (string.IsNullOrWhiteSpace(news.Title))
                return (false, "Tiêu đề bài viết không được để trống.");

            if (string.IsNullOrWhiteSpace(news.Slug))
                return (false, "Slug không được để trống.");

            if (string.IsNullOrWhiteSpace(news.Content))
                return (false, "Nội dung bài viết không được để trống.");

            if (await _newsRepository.SlugExistsAsync(news.Slug, news.Id))
                return (false, $"Slug '{news.Slug}' đã tồn tại. Vui lòng chọn slug khác.");

            if (news.IsPublished && existing.PublishedAt == null)
                news.PublishedAt = DateTime.Now;

            if (news.IsPublished && existing.PublishedAt != null)
                news.PublishedAt = existing.PublishedAt;

            if (!news.IsPublished)
                news.PublishedAt = null;

            await _newsRepository.UpdateAsync(news);
            return (true, "Cập nhật bài viết thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var existing = await _newsRepository.GetByIdAsync(id);
            if (existing == null)
                return (false, "Bài viết không tồn tại.");

            await _newsRepository.DeleteAsync(id);
            return (true, "Xoá bài viết thành công.");
        }


        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            return await _newsRepository.SlugExistsAsync(slug, excludeId);
        }
    }
}