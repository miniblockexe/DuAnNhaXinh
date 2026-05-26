using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<Order?> GetByCodeAsync(string orderCode)
        {
            return await _orderRepository.GetByCodeAsync(orderCode);
        }

        public async Task<Order?> GetWithDetailsAsync(int id)
        {
            return await _orderRepository.GetWithDetailsAsync(id);
        }

        public async Task<List<Order>> GetOrdersByUserAsync(string userId)
        {
            return await _orderRepository.GetByUserIdAsync(userId);
        }

        public async Task<(List<Order> Items, int TotalCount, int TotalPages)> GetPagedOrdersAsync(
            int page,
            int pageSize,
            OrderStatus? status = null,
            string? keyword = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, totalCount) = await _orderRepository.GetPagedAsync(page, pageSize, status, keyword);
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

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
            if (cartItems == null || !cartItems.Any())
                return (false, "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi đặt hàng.", null);

            foreach (var item in cartItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);

                if (product == null || !product.IsActive)
                    return (false, $"Sản phẩm '{item.ProductName}' không còn tồn tại.", null);

                if (product.StockQuantity < item.Quantity)
                    return (false, $"Sản phẩm '{item.ProductName}' không đủ hàng. Còn lại: {product.StockQuantity}.", null);
            }

            var orderCode = await _orderRepository.GenerateOrderCodeAsync();

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

            await _orderRepository.AddAsync(order);

            foreach (var item in cartItems)
            {
                await _productRepository.UpdateStockAsync(item.ProductId, item.Quantity);
            }

            return (true, "Đặt hàng thành công!", orderCode);
        }

        public async Task<(bool Success, string Message)> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return (false, "Đơn hàng không tồn tại.");

            if (order.Status == OrderStatus.Cancelled)
                return (false, "Không thể cập nhật đơn hàng đã bị huỷ.");

            if (order.Status == OrderStatus.Delivered)
                return (false, "Đơn hàng đã giao thành công, không thể thay đổi trạng thái.");

            await _orderRepository.UpdateStatusAsync(orderId, newStatus);
            return (true, "Cập nhật trạng thái đơn hàng thành công.");
        }

        public async Task<string> GenerateOrderCodeAsync()
        {
            return await _orderRepository.GenerateOrderCodeAsync();
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            var stats = new DashboardStats
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

            return stats;
        }
    }
}
