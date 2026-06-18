using Microsoft.AspNetCore.Mvc;
using NhaXinh.Services.Interfaces;
using NhaXinh.ViewModels.News;

namespace NhaXinh.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private const int PageSize = 9;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        public async Task<IActionResult> Index(int page = 1, string? keyword = null)
        {
            var (items, totalCount, totalPages) =
                await _newsService.GetPublishedPagedAsync(page, PageSize, keyword);

            var featured = await _newsService.GetFeaturedAsync(5);

            var model = new NewsListViewModel
            {
                NewsList = items,
                FeaturedNews = featured,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalCount = totalCount,
                SearchKeyword = keyword
            };

            return View(model);
        }

        // GET: /News/Detail/slug-bai-viet
        public async Task<IActionResult> Detail(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return NotFound();

            var news = await _newsService.GetBySlugAsync(slug);
            if (news is null)
                return NotFound();

            var related = await _newsService.GetRelatedAsync(news.Id, count: 4);

            return View(new NewsDetailViewModel
            {
                News = news,
                RelatedNews = related
            });
        }
    }
}
