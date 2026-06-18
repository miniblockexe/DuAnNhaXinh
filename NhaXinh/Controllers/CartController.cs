using Microsoft.AspNetCore.Mvc;
using NhaXinh.Services.Interfaces;
using NhaXinh.ViewModels.Cart;

namespace NhaXinh.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        // GET: /Cart
        public IActionResult Index()
        {
            var vm = new CartViewModel
            {
                Items = _cartService.GetCart(),
                TotalAmount = _cartService.GetTotalAmount(),
                TotalItems = _cartService.GetTotalItems()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1, bool buyNow = false)
        {
            var product = await _productService.GetWithImagesAsync(productId);

            if (product is null || !product.IsActive)
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });

            if (!await _productService.IsInStockAsync(productId, quantity))
                return Json(new { success = false, message = "Sản phẩm không đủ số lượng trong kho." });

            if (buyNow)
            {
                TempData["BuyNowProductId"] = product.Id;
                TempData["BuyNowProductName"] = product.Name;
                TempData["BuyNowProductImage"] = product.MainImageUrl ?? "";
                TempData["BuyNowUnitPrice"] = (product.DiscountPrice ?? product.Price).ToString();
                TempData["BuyNowQuantity"] = quantity;
                return Json(new { success = true, redirectUrl = Url.Action("Checkout", "Order") });
            }

            // Chỉ add vào giỏ khi KHÔNG phải mua ngay
            _cartService.AddToCart(product, quantity);

            return Json(new
            {
                success = true,
                cartCount = _cartService.GetCart().Count
            });
        }

        // POST: /Cart/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int productId, int quantity)
        {
            _cartService.UpdateQuantity(productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            _cartService.RemoveFromCart(productId);
            TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            _cartService.ClearCart();
            return RedirectToAction(nameof(Index));
        }
    }
}
