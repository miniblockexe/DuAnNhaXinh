using Microsoft.EntityFrameworkCore;
using NhaXinh.Data;
using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;

namespace NhaXinh.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetByCodeAsync(string orderCode)
        {
            return await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }

        public async Task<Order?> GetWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<(List<Order> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            OrderStatus? status = null, string? keyword = null)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(o =>
                    o.OrderCode.ToLower().Contains(kw) ||
                    o.ReceiverName.ToLower().Contains(kw) ||
                    o.ReceiverPhone.Contains(kw));
            }

            query = query.OrderByDescending(o => o.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(Order order)
        {
            order.CreatedAt = DateTime.Now;
            order.UpdatedAt = DateTime.Now;
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return;

            order.Status = status;
            order.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetCountByStatusAsync(OrderStatus status)
        {
            return await _context.Orders.CountAsync(o => o.Status == status);
        }

        public async Task<List<Order>> GetRecentAsync(int count = 10)
        {
            return await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
        public async Task<string> GenerateOrderCodeAsync()
        {
            var year = DateTime.Now.Year;
            var prefix = $"NX{year}";

            var countThisYear = await _context.Orders
                .CountAsync(o => o.OrderCode.StartsWith(prefix));

            return $"{prefix}{(countThisYear + 1):D4}";
        }
    }
}
