using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _bannerRepository;

        public BannerService(IBannerRepository bannerRepository)
        {
            _bannerRepository = bannerRepository;
        }

        public async Task<List<Banner>> GetAllAsync()
        {
            return await _bannerRepository.GetAllAsync();
        }

        public async Task<List<Banner>> GetActiveAsync()
        {
            return await _bannerRepository.GetActiveAsync();
        }

        public async Task<Banner?> GetByIdAsync(int id)
        {
            return await _bannerRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> CreateAsync(Banner banner)
        {
            if (string.IsNullOrWhiteSpace(banner.Title))
                return (false, "Tiêu đề banner không được để trống.");

            if (string.IsNullOrWhiteSpace(banner.ImageUrl))
                return (false, "Hình ảnh banner không được để trống.");

            if (banner.DisplayOrder <= 0)
            {
                var all = await _bannerRepository.GetAllAsync();
                banner.DisplayOrder = all.Count + 1;
            }

            await _bannerRepository.AddAsync(banner);
            return (true, "Thêm banner thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Banner banner)
        {
            var existing = await _bannerRepository.GetByIdAsync(banner.Id);
            if (existing == null)
                return (false, "Banner không tồn tại.");

            if (string.IsNullOrWhiteSpace(banner.Title))
                return (false, "Tiêu đề banner không được để trống.");

            if (string.IsNullOrWhiteSpace(banner.ImageUrl))
                return (false, "Hình ảnh banner không được để trống.");

            await _bannerRepository.UpdateAsync(banner);
            return (true, "Cập nhật banner thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var existing = await _bannerRepository.GetByIdAsync(id);
            if (existing == null)
                return (false, "Banner không tồn tại.");

            await _bannerRepository.DeleteAsync(id);
            return (true, "Xoá banner thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateDisplayOrderAsync(
            List<(int BannerId, int Order)> orderList)
        {
            if (orderList == null || !orderList.Any())
                return (false, "Danh sách sắp xếp trống.");

            foreach (var (bannerId, order) in orderList)
            {
                var banner = await _bannerRepository.GetByIdAsync(bannerId);
                if (banner == null) continue;

                banner.DisplayOrder = order;
                await _bannerRepository.UpdateAsync(banner);
            }

            return (true, "Cập nhật thứ tự banner thành công.");
        }
    }
}