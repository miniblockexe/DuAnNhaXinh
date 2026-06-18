using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhaXinh.Models;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BannerController : Controller
    {
        private readonly IBannerService _bannerService;
        private readonly IFileService _fileService;

        public BannerController(IBannerService bannerService, IFileService fileService)
        {
            _bannerService = bannerService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var banners = await _bannerService.GetAllAsync();
            return View(banners.OrderBy(b => b.DisplayOrder).ToList());
        }

        [HttpGet]
        public IActionResult Create() => View(new Banner());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Banner banner, IFormFile? imageFile)
        {
            ModelState.Remove("imageFile");

            if (imageFile is null || imageFile.Length == 0)
                ModelState.AddModelError("imageFile", "Vui lòng chọn ảnh banner.");

            if (!ModelState.IsValid) return View(banner);

            (bool saved, string saveMsg, string? filePath) =
                await _fileService.SaveImageAsync(imageFile!, ImageFolder.Banners);

            if (!saved || filePath is null)
            {
                ModelState.AddModelError("imageFile", saveMsg);
                return View(banner);
            }

            banner.ImageUrl = filePath;
            var result = await _bannerService.CreateAsync(banner);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(banner);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var banner = await _bannerService.GetByIdAsync(id);
            if (banner is null) return NotFound();
            return View(banner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Banner banner, IFormFile? imageFile, string? existingImage)
        {
            if (id != banner.Id) return NotFound();
            ModelState.Remove("imageFile");
            ModelState.Remove("existingImage");

            if (!ModelState.IsValid) return View(banner);

            if (imageFile is not null && imageFile.Length > 0)
            {
                (bool saved, string saveMsg, string? filePath) =
                    await _fileService.SaveImageAsync(imageFile, ImageFolder.Banners);

                if (!saved || filePath is null)
                {
                    ModelState.AddModelError("imageFile", saveMsg);
                    return View(banner);
                }

                banner.ImageUrl = filePath;
            }
            else
            {
                banner.ImageUrl = existingImage ?? banner.ImageUrl;
            }

            var result = await _bannerService.UpdateAsync(banner);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(banner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bannerService.DeleteAsync(id);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var banner = await _bannerService.GetByIdAsync(id);
            if (banner is null)
                return Json(new { success = false });

            banner.IsActive = !banner.IsActive;
            var result = await _bannerService.UpdateAsync(banner);

            return Json(new { success = result.Success, isActive = banner.IsActive });
        }
    }
}
