using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NhaXinh.Models;
using NhaXinh.Services.Interfaces;
using NhaXinh.ViewModels.Home;

namespace NhaXinh.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBannerService _bannerService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(
            IBannerService bannerService,
            IProductService productService,
            ICategoryService categoryService)
        {
            _bannerService = bannerService;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                Banners = await _bannerService.GetActiveAsync(),
                FeaturedProducts = await _productService.GetFeaturedAsync(6),
                Categories = await _categoryService.GetAllActiveAsync()
            };
            return View(vm);
        }

        public IActionResult Privacy() => View();
    }
}
