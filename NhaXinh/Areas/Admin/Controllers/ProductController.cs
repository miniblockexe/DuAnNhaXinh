using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhaXinh.Helpers;
using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;
        private readonly IProductImageRepository _productImageRepository;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IFileService fileService,
            IProductImageRepository productImageRepository)
        {
            _productService = productService;
            _categoryService = categoryService;
            _fileService = fileService;
            _productImageRepository = productImageRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, int? categoryId, int page = 1)
        {
            const int pageSize = 10;

            var (items, totalCount, totalPages) = await _productService.GetPagedAsync(
                page, pageSize, categoryId: categoryId, keyword: keyword);

            ViewBag.Keyword = keyword;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = await _categoryService.GetAllAsync();
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model, IFormFile? imageFile, List<IFormFile>? subImages)
        {
            ModelState.Remove("Category");
            ModelState.Remove("ProductImages");
            ModelState.Remove("imageFile");
            ModelState.Remove("subImages");

            if (string.IsNullOrWhiteSpace(model.Slug))
                model.Slug = SlugHelper.GenerateSlug(model.Name);

            if (imageFile is not null && imageFile.Length > 0)
            {
                (bool ok, string msg, string? path) =
                    await _fileService.SaveImageAsync(imageFile, ImageFolder.Products);

                if (!ok)
                {
                    ModelState.AddModelError("imageFile", msg);
                    ViewBag.Categories = await _categoryService.GetAllAsync();
                    return View(model);
                }

                model.MainImageUrl = path;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(model);
            }

            var (success, message) = await _productService.CreateAsync(model);

            if (success && subImages is not null && subImages.Count > 0)
            {
                var productImages = new List<ProductImage>();
                int order = 0;
                foreach (var file in subImages)
                {
                    if (file.Length == 0) continue;
                    var (ok, _, subPath) = await _fileService.SaveImageAsync(file, ImageFolder.Products);
                    if (ok && subPath is not null)
                        productImages.Add(new ProductImage
                        {
                            ProductId = model.Id,
                            ImageUrl = subPath,
                            DisplayOrder = order++
                        });
                }
                if (productImages.Count > 0)
                    await _productImageRepository.AddRangeAsync(productImages);
            }

            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, message);
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetWithImagesAsync(id);
            if (product is null) return NotFound();

            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Product model,
            IFormFile? imageFile,
            string? existingImageUrl,
            List<IFormFile>? subImages)
        {
            if (id != model.Id) return BadRequest();

            ModelState.Remove("Category");
            ModelState.Remove("ProductImages");
            ModelState.Remove("imageFile");
            ModelState.Remove("existingImageUrl");
            ModelState.Remove("subImages");

            if (string.IsNullOrWhiteSpace(model.Slug))
                model.Slug = SlugHelper.GenerateSlug(model.Name);

            if (imageFile is not null && imageFile.Length > 0)
            {
                (bool ok, string msg, string? path) =
                    await _fileService.SaveImageAsync(imageFile, ImageFolder.Products);

                if (!ok)
                {
                    ModelState.AddModelError("imageFile", msg);
                    model.MainImageUrl = existingImageUrl;
                    ViewBag.Categories = await _categoryService.GetAllAsync();
                    return View(model);
                }

                _fileService.DeleteImage(existingImageUrl);
                model.MainImageUrl = path;
            }
            else
            {
                model.MainImageUrl = existingImageUrl;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(model);
            }

            var (success, message) = await _productService.UpdateAsync(model);

            if (success && subImages is not null && subImages.Count > 0)
            {
                var existing = await _productImageRepository.GetByProductIdAsync(id);
                int order = existing.Count > 0 ? existing.Max(x => x.DisplayOrder) + 1 : 0;

                var newImages = new List<ProductImage>();
                foreach (var file in subImages)
                {
                    if (file.Length == 0) continue;
                    var (ok, _, subPath) = await _fileService.SaveImageAsync(file, ImageFolder.Products);
                    if (ok && subPath is not null)
                        newImages.Add(new ProductImage
                        {
                            ProductId = id,
                            ImageUrl = subPath,
                            DisplayOrder = order++
                        });
                }
                if (newImages.Count > 0)
                    await _productImageRepository.AddRangeAsync(newImages);
            }

            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, message);
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubImage(int imageId, int productId)
        {
            var img = await _productImageRepository.GetByIdAsync(imageId);
            if (img is not null)
            {
                _fileService.DeleteImage(img.ImageUrl);
                await _productImageRepository.DeleteAsync(imageId);
            }
            return RedirectToAction(nameof(Edit), new { id = productId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (success, message) = await _productService.DeleteAsync(id);

            TempData[success ? "Success" : "Error"] = message;

            return success
                ? RedirectToAction(nameof(Index))
                : RedirectToAction(nameof(Delete), new { id });
        }
    }
}
