using NhaXinh.Models;

namespace NhaXinh.Services.Interfaces
{
    public interface IBannerService
    {
        Task<List<Banner>> GetAllAsync();

        Task<List<Banner>> GetActiveAsync();

        Task<Banner?> GetByIdAsync(int id);

        Task<(bool Success, string Message)> CreateAsync(Banner banner);
        Task<(bool Success, string Message)> UpdateAsync(Banner banner);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<(bool Success, string Message)> UpdateDisplayOrderAsync(List<(int BannerId, int Order)> orderList);
    }
}