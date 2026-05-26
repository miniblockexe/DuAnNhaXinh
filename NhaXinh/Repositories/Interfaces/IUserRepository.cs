using NhaXinh.Models;

namespace NhaXinh.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);

        Task<(List<ApplicationUser> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string? keyword = null
        );

        Task UpdateAsync(ApplicationUser user);
        Task SetActiveStatusAsync(string id, bool isActive);

        Task<int> GetTotalCountAsync();
    }
}
