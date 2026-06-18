using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhaXinh.Models;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, OrderStatus? status = null, string? keyword = null)
        {
            const int pageSize = 10;

            var (items, totalCount, totalPages) =
                await _orderService.GetPagedOrdersAsync(page, pageSize, status, keyword);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.Status = status;
            ViewBag.Keyword = keyword;

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderService.GetWithDetailsAsync(id);
            if (order is null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus newStatus)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Detail), new { id = orderId });
        }
    }
}
