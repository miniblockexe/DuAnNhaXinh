using NhaXinh.Models;

namespace NhaXinh.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByCodeAsync(string orderCode);
        Task<Order?> GetWithDetailsAsync(int id);

        Task<List<Order>> GetByUserIdAsync(string userId);

        Task<(List<Order> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            OrderStatus? status = null,
            string? keyword = null
        );

        Task AddAsync(Order order);
        Task UpdateStatusAsync(int id, OrderStatus status);

        Task<int> GetTotalCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetCountByStatusAsync(OrderStatus status);
        Task<List<Order>> GetRecentAsync(int count = 10);

        Task<string> GenerateOrderCodeAsync();
    }
}
