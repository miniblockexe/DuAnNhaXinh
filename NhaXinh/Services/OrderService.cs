using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Order?> GetByIdAsync(int id)
            => await _orderRepository.GetByIdAsync(id);

        public async Task<Order?> GetByCodeAsync(string orderCode)
            => await _orderRepository.GetByCodeAsync(orderCode);

        public async Task<Order?> GetWithDetailsAsync(int id)
            => await _orderRepository.GetWithDetailsAsync(id);

        public async Task<List<Order>> GetOrdersByUserAsync(string userId)
            => await _orderRepository.GetByUserIdAsync(userId);

        public async Task<(List<Order> Items, int TotalCount, int TotalPages)> GetPagedOrdersAsync(
            int page, int pageSize, OrderStatus? status = null, string? keyword = null)
        {
            page = Math.Max(1, page);
            pageSize = pageSize < 1 ? 10 : pageSize;

            var (items, totalCount) = await _orderRepository.GetPagedAsync(page, pageSize, status, keyword);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return (items, totalCount, totalPages);
        }

        public async Task<(bool Success, string Message, string? OrderCode)> PlaceOrderAsync(
            string userId,
            string receiverName,
            string receiverPhone,
            string shippingAddress,
            string? note,
            PaymentMethod paymentMethod,
            List<CartItem> cartItems)
        {
            if (cartItems is null || cartItems.Count == 0)
                return (false, "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi đặt hàng.", null);

            var stockError = await ValidateStockAsync(cartItems);
            if (stockError is not null)
                return (false, stockError, null);

            var orderCode = await _orderRepository.GenerateOrderCodeAsync();
            var order = BuildOrder(userId, receiverName, receiverPhone,
                                       shippingAddress, note, paymentMethod,
                                       orderCode, cartItems);

            await _orderRepository.AddAsync(order);
            await DeductStockAsync(cartItems);

            _logger.LogInformation(
                "Đơn hàng {OrderCode} tạo thành công. UserId={UserId}", orderCode, userId);

            return (true, "Đặt hàng thành công!", orderCode);
        }

        public async Task<(bool Success, string Message)> UpdateOrderStatusAsync(
            int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null)
                return (false, "Đơn hàng không tồn tại.");

            if (!IsValidTransition(order.Status, newStatus))
            {
                _logger.LogWarning(
                    "Chuyển trạng thái không hợp lệ: {From} → {To}. OrderId={OrderId}",
                    order.Status, newStatus, orderId);

                return (false, $"Không thể chuyển từ '{order.Status}' sang '{newStatus}'.");
            }

            await _orderRepository.UpdateStatusAsync(orderId, newStatus);
            return (true, "Cập nhật trạng thái đơn hàng thành công.");
        }

        public async Task<string> GenerateOrderCodeAsync()
            => await _orderRepository.GenerateOrderCodeAsync();

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            return new DashboardStats
            {
                TotalOrders = await _orderRepository.GetTotalCountAsync(),
                TotalRevenue = await _orderRepository.GetTotalRevenueAsync(),
                PendingOrders = await _orderRepository.GetCountByStatusAsync(OrderStatus.Pending),
                ConfirmedOrders = await _orderRepository.GetCountByStatusAsync(OrderStatus.Confirmed),
                ShippingOrders = await _orderRepository.GetCountByStatusAsync(OrderStatus.Shipping),
                DeliveredOrders = await _orderRepository.GetCountByStatusAsync(OrderStatus.Delivered),
                CancelledOrders = await _orderRepository.GetCountByStatusAsync(OrderStatus.Cancelled),
                RecentOrders = await _orderRepository.GetRecentAsync(count: 10)
            };
        }


        private static Order BuildOrder(
            string userId, string receiverName, string receiverPhone,
            string shippingAddress, string? note, PaymentMethod paymentMethod,
            string orderCode, List<CartItem> cartItems)
        {
            var order = new Order
            {
                OrderCode = orderCode,
                UserId = userId,
                ReceiverName = receiverName,
                ReceiverPhone = receiverPhone,
                ShippingAddress = shippingAddress,
                Note = note,
                TotalAmount = cartItems.Sum(x => x.SubTotal),
                Status = OrderStatus.Pending,
                PaymentMethod = paymentMethod,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            foreach (var item in cartItems)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductImage = item.ProductImage,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    SubTotal = item.SubTotal
                });
            }

            return order;
        }

        private async Task<string?> ValidateStockAsync(List<CartItem> cartItems)
        {
            foreach (var item in cartItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);

                if (product is null || !product.IsActive)
                    return $"Sản phẩm '{item.ProductName}' không còn tồn tại.";

                if (product.StockQuantity < item.Quantity)
                    return $"Sản phẩm '{item.ProductName}' không đủ hàng. Còn lại: {product.StockQuantity}.";
            }

            return null;
        }

        private async Task DeductStockAsync(List<CartItem> cartItems)
        {
            foreach (var item in cartItems)
                await _productRepository.UpdateStockAsync(item.ProductId, item.Quantity);
        }

        private static bool IsValidTransition(OrderStatus current, OrderStatus next)
            => (current, next) switch
            {
                (OrderStatus.Pending, OrderStatus.Confirmed) => true,
                (OrderStatus.Pending, OrderStatus.Cancelled) => true,
                (OrderStatus.Confirmed, OrderStatus.Shipping) => true,
                (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
                (OrderStatus.Shipping, OrderStatus.Delivered) => true,
                _ => false
            };
    }
}