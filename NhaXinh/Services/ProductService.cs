using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product?> GetBySlugAsync(string slug)
        {
            return await _productRepository.GetBySlugAsync(slug);
        }

        public async Task<Product?> GetWithImagesAsync(int id)
        {
            return await _productRepository.GetWithImagesAsync(id);
        }

        public async Task<(List<Product> Items, int TotalCount, int TotalPages)> GetPagedAsync(
            int page,
            int pageSize,
            int? categoryId = null,
            string? keyword = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 12;

            var (items, totalCount) = await _productRepository.GetPagedAsync(
                page, pageSize, categoryId, keyword, minPrice, maxPrice, sortBy);

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return (items, totalCount, totalPages);
        }

        public async Task<List<Product>> GetFeaturedAsync(int count = 6)
        {
            return await _productRepository.GetFeaturedAsync(count);
        }

        public async Task<List<Product>> GetRelatedAsync(int productId, int categoryId, int count = 4)
        {
            return await _productRepository.GetRelatedAsync(productId, categoryId, count);
        }

        public async Task<List<Product>> GetLowStockAsync(int threshold = 5)
        {
            return await _productRepository.GetLowStockAsync(threshold);
        }


        public async Task<(bool Success, string Message)> CreateAsync(Product product)
        {
            if (await _productRepository.SlugExistsAsync(product.Slug))
                return (false, $"Slug '{product.Slug}' đã tồn tại. Vui lòng chọn slug khác.");

            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            if (category == null)
                return (false, "Danh mục sản phẩm không tồn tại.");

            if (product.Price < 0)
                return (false, "Giá sản phẩm không hợp lệ.");

            if (product.DiscountPrice.HasValue && product.DiscountPrice >= product.Price)
                return (false, "Giá khuyến mãi phải nhỏ hơn giá gốc.");

            if (product.StockQuantity < 0)
                return (false, "Số lượng tồn kho không hợp lệ.");

            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;

            await _productRepository.AddAsync(product);
            return (true, "Thêm sản phẩm thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Product product)
        {
            var existing = await _productRepository.GetByIdAsync(product.Id);
            if (existing == null)
                return (false, "Sản phẩm không tồn tại.");

            if (await _productRepository.SlugExistsAsync(product.Slug, product.Id))
                return (false, $"Slug '{product.Slug}' đã tồn tại. Vui lòng chọn slug khác.");

            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            if (category == null)
                return (false, "Danh mục sản phẩm không tồn tại.");

            if (product.Price < 0)
                return (false, "Giá sản phẩm không hợp lệ.");

            if (product.DiscountPrice.HasValue && product.DiscountPrice >= product.Price)
                return (false, "Giá khuyến mãi phải nhỏ hơn giá gốc.");

            if (product.StockQuantity < 0)
                return (false, "Số lượng tồn kho không hợp lệ.");

            product.UpdatedAt = DateTime.Now;

            await _productRepository.UpdateAsync(product);
            return (true, "Cập nhật sản phẩm thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return (false, "Sản phẩm không tồn tại.");

            await _productRepository.DeleteAsync(id);
            return (true, "Xóa sản phẩm thành công.");
        }

        public async Task IncrementViewCountAsync(int id)
        {
            await _productRepository.IncrementViewCountAsync(id);
        }

        public async Task<bool> IsInStockAsync(int productId, int requiredQuantity = 1)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return false;

            return product.StockQuantity >= requiredQuantity;
        }

        public async Task<(bool Success, string Message)> ReduceStockAsync(int productId, int quantity)
        {
            if (quantity <= 0)
                return (false, "Số lượng cần trừ phải lớn hơn 0.");

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return (false, "Sản phẩm không tồn tại.");

            if (product.StockQuantity < quantity)
                return (false, $"Sản phẩm '{product.Name}' không đủ hàng. Còn lại: {product.StockQuantity}.");

            await _productRepository.UpdateStockAsync(productId, quantity);
            return (true, "Cập nhật tồn kho thành công.");
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            return await _productRepository.SlugExistsAsync(slug, excludeId);
        }
    }
}
