using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhaXinh.Helpers;
using NhaXinh.Models;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private readonly IFileService _fileService;
        private const int PageSize = 10;

        public NewsController(INewsService newsService, IFileService fileService)
        {
            _newsService = newsService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? keyword = null)
        {
            var (items, totalCount, totalPages) = await _newsService.GetAllPagedAsync(page, PageSize, keyword);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.Keyword = keyword;

            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
            => View(new News { Author = User.Identity?.Name });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News news, IFormFile? thumbnailFile)
        {
            ModelState.Remove("thumbnailFile");

            if (string.IsNullOrWhiteSpace(news.Slug))
                news.Slug = SlugHelper.GenerateSlug(news.Title);

            if (await _newsService.SlugExistsAsync(news.Slug))
                ModelState.AddModelError("Slug", "Slug này đã tồn tại, vui lòng chọn slug khác.");

            if (thumbnailFile is not null && thumbnailFile.Length > 0)
            {
                var (isValid, validMsg) = _fileService.IsValidImage(thumbnailFile);
                if (!isValid)
                    ModelState.AddModelError("thumbnailFile", validMsg);
            }

            if (!ModelState.IsValid) return View(news);

            if (thumbnailFile is not null && thumbnailFile.Length > 0)
            {
                (bool saved, string saveMsg, string? filePath) =
                    await _fileService.SaveImageAsync(thumbnailFile, ImageFolder.News);

                if (!saved || filePath is null)
                {
                    ModelState.AddModelError("thumbnailFile", saveMsg);
                    return View(news);
                }

                news.ThumbnailUrl = filePath;
            }

            var result = await _newsService.CreateAsync(news);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(news);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var news = await _newsService.GetByIdAsync(id);
            if (news is null) return NotFound();
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, News news, IFormFile? thumbnailFile, string? existingThumbnail)
        {
            if (id != news.Id) return NotFound();

            ModelState.Remove("thumbnailFile");
            ModelState.Remove("existingThumbnail");

            if (string.IsNullOrWhiteSpace(news.Slug))
                news.Slug = SlugHelper.GenerateSlug(news.Title);

            if (await _newsService.SlugExistsAsync(news.Slug, excludeId: id))
                ModelState.AddModelError("Slug", "Slug này đã tồn tại, vui lòng chọn slug khác.");

            if (thumbnailFile is not null && thumbnailFile.Length > 0)
            {
                var (isValid, validMsg) = _fileService.IsValidImage(thumbnailFile);
                if (!isValid)
                    ModelState.AddModelError("thumbnailFile", validMsg);
            }

            if (!ModelState.IsValid) return View(news);

            if (thumbnailFile is not null && thumbnailFile.Length > 0)
            {
                (bool saved, string saveMsg, string? filePath) =
                    await _fileService.SaveImageAsync(thumbnailFile, ImageFolder.News);

                if (!saved || filePath is null)
                {
                    ModelState.AddModelError("thumbnailFile", saveMsg);
                    return View(news);
                }
                _fileService.DeleteImage(existingThumbnail);
                news.ThumbnailUrl = filePath;
            }
            else
            {
                news.ThumbnailUrl = existingThumbnail;
            }

            var result = await _newsService.UpdateAsync(news);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _newsService.GetByIdAsync(id);

            var result = await _newsService.DeleteAsync(id);

            if (result.Success && news is not null)
                _fileService.DeleteImage(news.ThumbnailUrl);

            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
