using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NhaXinh.Helpers;
using NhaXinh.Models;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;

        public CategoryController(ICategoryService categoryService, IFileService fileService)
        {
            _categoryService = categoryService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetParentCategoriesWithChildrenAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadParentSelectListAsync();
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile? imageFile)
        {
            ModelState.Remove("imageFile");
            ModelState.Remove("ParentCategory");
            ModelState.Remove("SubCategories");
            ModelState.Remove("Products");

            if (!ModelState.IsValid)
            {
                await LoadParentSelectListAsync();
                return View(category);
            }

            if (string.IsNullOrWhiteSpace(category.Slug))
                category.Slug = SlugHelper.GenerateSlug(category.Name);

            if (imageFile is not null && imageFile.Length > 0)
            {
                (bool saved, string saveMsg, string? filePath) =
                    await _fileService.SaveImageAsync(imageFile, ImageFolder.Categories);

                if (!saved || filePath is null)
                {
                    ModelState.AddModelError("imageFile", saveMsg);
                    await LoadParentSelectListAsync();
                    return View(category);
                }

                category.ImageUrl = filePath;
            }

            var result = await _categoryService.CreateAsync(category);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            await LoadParentSelectListAsync();
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null) return NotFound();

            await LoadParentSelectListAsync(excludeId: id, selectedId: category.ParentId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category, IFormFile? imageFile, string? existingImage)
        {
            if (id != category.Id) return NotFound();

            ModelState.Remove("imageFile");
            ModelState.Remove("existingImage");
            ModelState.Remove("ParentCategory");
            ModelState.Remove("SubCategories");
            ModelState.Remove("Products");

            if (!ModelState.IsValid)
            {
                await LoadParentSelectListAsync(excludeId: id, selectedId: category.ParentId);
                return View(category);
            }

            if (imageFile is not null && imageFile.Length > 0)
            {
                (bool saved, string saveMsg, string? filePath) =
                    await _fileService.SaveImageAsync(imageFile, ImageFolder.Categories);

                if (!saved || filePath is null)
                {
                    ModelState.AddModelError("imageFile", saveMsg);
                    await LoadParentSelectListAsync(excludeId: id, selectedId: category.ParentId);
                    return View(category);
                }

                category.ImageUrl = filePath;
            }
            else
            {
                category.ImageUrl = existingImage;
            }

            var result = await _categoryService.UpdateAsync(category);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            await LoadParentSelectListAsync(excludeId: id, selectedId: category.ParentId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }


        private async Task LoadParentSelectListAsync(int? excludeId = null, int? selectedId = null)
        {
            var parents = await _categoryService.GetParentCategoriesAsync();

            var filtered = excludeId.HasValue
                ? parents.Where(c => c.Id != excludeId.Value).ToList()
                : parents;

            ViewBag.ParentCategories = new SelectList(filtered, "Id", "Name", selectedId);
        }
    }
}
