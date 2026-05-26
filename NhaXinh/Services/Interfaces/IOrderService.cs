using NhaXinh.Models;

namespace NhaXinh.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByCodeAsync(string orderCode);

        Task<Order?> GetWithDetailsAsync(int id);

        Task<List<Order>> GetOrdersByUserAsync(string userId);

        Task<(List<Order> Items, int TotalCount, int TotalPages)> GetPagedOrdersAsync(
            int page,
            int pageSize,
            OrderStatus? status = null,
            string? keyword = null
        );

        Task<(bool Success, string Message, string? OrderCode)> PlaceOrderAsync(
            string userId,
            string receiverName,
            string receiverPhone,
            string shippingAddress,
            string? note,
            PaymentMethod paymentMethod,
            List<CartItem> cartItems
        );

        Task<(bool Success, string Message)> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);

        Task<string> GenerateOrderCodeAsync();

        Task<DashboardStats> GetDashboardStatsAsync();
    }
    public class DashboardStats
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public List<Order> RecentOrders { get; set; } = new();
    }
}
