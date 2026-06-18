using Microsoft.AspNetCore.Mvc;
using NhaXinh.Services.Interfaces;
using NhaXinh.ViewModels.Product;

namespace NhaXinh.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: /Product?categoryId=&keyword=&sortBy=&minPrice=&maxPrice=&page=
        [HttpGet]
        public async Task<IActionResult> Index(
            int? categoryId = null,
            string? keyword = null,
            string? sortBy = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1)
        {
            const int pageSize = 12;

            var (items, totalCount, totalPages) = await _productService.GetPagedAsync(
                page, pageSize, categoryId, keyword, minPrice, maxPrice, sortBy);

            var vm = new ProductListViewModel
            {
                Products = items,
                Categories = await _categoryService.GetAllActiveAsync(),
                CurrentPage = page,
                TotalPages = totalPages,
                TotalCount = totalCount,
                CategoryId = categoryId,
                Keyword = keyword,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            return View(vm);
        }

        // GET: /Product/Detail/{slug}
        [HttpGet]
        public async Task<IActionResult> Detail(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return NotFound();

            var product = await _productService.GetBySlugAsync(slug);
            if (product is null)
                return NotFound();

            var productWithImages = await _productService.GetWithImagesAsync(product.Id);
            if (productWithImages is null)
                return NotFound();

            await _productService.IncrementViewCountAsync(productWithImages.Id);

            return View(new ProductDetailViewModel
            {
                Product = productWithImages,
                RelatedProducts = await _productService.GetRelatedAsync(
                    productWithImages.Id, productWithImages.CategoryId, count: 4)
            });
        }
    }
}
