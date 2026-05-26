using NhaXinh.Models;

namespace NhaXinh.Repositories.Interfaces
{
    public interface IBannerRepository
    {
        Task<List<Banner>> GetAllAsync();
        Task<List<Banner>> GetActiveAsync();

        Task<Banner?> GetByIdAsync(int id);

        Task AddAsync(Banner banner);
        Task UpdateAsync(Banner banner);
        Task DeleteAsync(int id);
    }
}
