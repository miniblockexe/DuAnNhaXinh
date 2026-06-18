using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NhaXinh.Extensions;
using NhaXinh.Models;
using NhaXinh.Services.Interfaces;
using NhaXinh.ViewModels.Order;
using Microsoft.AspNetCore.Authorization;

namespace NhaXinh.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(
            IOrderService orderService,
            ICartService cartService,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _cartService = cartService;
            _emailService = emailService;
            _userManager = userManager;
        }

        // GET: /Order/Checkout
        [HttpGet]
        public async Task<IActionResult> Checkout(string? selectedIds = null)
        {
            List<CartItem> cartItems;

            if (TempData["BuyNowProductId"] is int buyNowId)
            {
                var unitPrice = decimal.TryParse(TempData["BuyNowUnitPrice"]?.ToString(), out var p) ? p : 0;
                var qty = TempData["BuyNowQuantity"] is int q ? q : 1;

                cartItems = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId    = buyNowId,
                        ProductName  = TempData["BuyNowProductName"]?.ToString() ?? "",
                        ProductImage = TempData["BuyNowProductImage"]?.ToString() ?? "",
                        UnitPrice    = unitPrice,
                        Quantity     = qty,
                    }
                };
            }
            else
            {
                var allItems = _cartService.GetCart();
                if (!allItems.Any())
                {
                    TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                    return RedirectToAction("Index", "Cart");
                }

                if (!string.IsNullOrEmpty(selectedIds))
                {
                    var ids = selectedIds
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => int.TryParse(s, out var id) ? id : 0)
                        .Where(id => id > 0)
                        .ToHashSet();
                    cartItems = allItems.Where(x => ids.Contains(x.ProductId)).ToList();
                }
                else
                {
                    cartItems = allItems;
                }
            }

            if (!cartItems.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm.";
                return RedirectToAction("Index", "Cart");
            }

            var user = await _userManager.GetUserAsync(User);

            return View(new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(x => x.SubTotal),
                ReceiverName = user?.FullName ?? string.Empty,
                ReceiverPhone = user?.PhoneNumber ?? string.Empty,
                ShippingAddress = user?.Address ?? string.Empty,
            });
        }

        // POST: /Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel vm, string? selectedIds = null)
        {
            if (!vm.CartItems.Any())
            {
                var allItems = _cartService.GetCart();
                var ids = (selectedIds ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.TryParse(s, out var id) ? id : 0)
                    .Where(id => id > 0)
                    .ToHashSet();

                vm.CartItems = ids.Any()
                    ? allItems.Where(x => ids.Contains(x.ProductId)).ToList()
                    : allItems;
            }

            vm.TotalAmount = vm.CartItems.Sum(x => x.SubTotal);

            if (!vm.CartItems.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm.";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid) return View(vm);

            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var (success, message, orderCode) = await _orderService.PlaceOrderAsync(
                userId,
                vm.ReceiverName,
                vm.ReceiverPhone,
                vm.ShippingAddress,
                vm.Note,
                vm.PaymentMethod,
                vm.CartItems);

            if (!success)
            {
                TempData["Error"] = message;
                return View(vm);
            }

            var cartProductIds = _cartService.GetCart().Select(x => x.ProductId).ToHashSet();
            foreach (var item in vm.CartItems)
            {
                if (cartProductIds.Contains(item.ProductId))
                    _cartService.RemoveFromCart(item.ProductId);
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var customerName = User.FindFirstValue(ClaimTypes.Name)
                            ?? User.FindFirstValue("FullName")
                            ?? vm.ReceiverName;
            var paymentLabel = vm.PaymentMethod switch
            {
                PaymentMethod.COD => "Thanh toán khi nhận hàng (COD)",
                PaymentMethod.BankTransfer => "Chuyển khoản ngân hàng",
                _ => vm.PaymentMethod.ToString()
            };

            _ = Task.Run(async () =>
            {
                if (!string.IsNullOrEmpty(userEmail))
                {
                    await _emailService.SendOrderConfirmationAsync(
                        toEmail: userEmail,
                        receiverName: vm.ReceiverName,
                        orderCode: orderCode!,
                        totalAmount: vm.TotalAmount,
                        phone: vm.ReceiverPhone,
                        address: vm.ShippingAddress,
                        paymentMethod: paymentLabel);
                }

                await _emailService.SendNewOrderNotificationAsync(
                    orderCode: orderCode!,
                    customerName: customerName,
                    totalAmount: vm.TotalAmount);
            });

            return RedirectToAction(nameof(Success), new { orderCode });
        }

        // GET: /Order/Success?orderCode=NX20240001
        [HttpGet]
        public async Task<IActionResult> Success(string orderCode)
        {
            if (string.IsNullOrWhiteSpace(orderCode))
                return RedirectToAction("Index", "Home");

            var order = await _orderService.GetByCodeAsync(orderCode);
            if (order is null)
                return RedirectToAction("Index", "Home");

            return View(new OrderDetailViewModel { Order = order });
        }

        // GET: /Order/History
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = User.GetUserId();
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return View(new OrderHistoryViewModel { Orders = orders });
        }

        // GET: /Order/Detail/5
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderService.GetWithDetailsAsync(id);
            if (order is null || order.UserId != User.GetUserId())
                return NotFound();

            return View(new OrderDetailViewModel { Order = order });
        }
    }
}
