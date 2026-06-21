using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class DashboardController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public DashboardController(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _orderService.GetDashboardStatsAsync();
            var lowStock = await _productService.GetLowStockAsync(threshold: 5);

            ViewBag.LowStockProducts = lowStock;

            return View(stats);
        }
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var orders = await _orderService.GetRecentPendingOrdersAsync(limit: 36);

            return Json(new
            {
                count = orders.Count,
                orders = orders.Select(o => new
                {
                    id = o.Id,
                    orderCode = o.OrderCode,
                    customerName = o.User?.FullName ?? "Khách hàng",
                    total = $"{o.TotalAmount:N0}đ",
                    createdAt = o.CreatedAt.ToString("dd/MM HH:mm")
                })
            });
        }
    }
}
