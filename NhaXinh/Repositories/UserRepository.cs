using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NhaXinh.Data;
using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;

namespace NhaXinh.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<(List<ApplicationUser> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? keyword = null)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(kw) ||
                    (u.Email != null && u.Email.ToLower().Contains(kw)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(kw)));
            }

            query = query.OrderByDescending(u => u.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task SetActiveStatusAsync(string id, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return;

            user.IsActive = isActive;
            await _userManager.UpdateAsync(user);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Users.CountAsync();
        }
    }
}
