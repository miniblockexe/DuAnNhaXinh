using System.ComponentModel.DataAnnotations;

namespace NhaXinh.ViewModels.Product
{
    public class ProductFilterViewModel
    {
        public string? Keyword { get; set; }

        public int? CategoryId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá tối thiểu không hợp lệ.")]
        public decimal? MinPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá tối đa không hợp lệ.")]
        public decimal? MaxPrice { get; set; }

        public string? SortBy { get; set; } = "newest";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        public bool HasFilter =>
            !string.IsNullOrWhiteSpace(Keyword) ||
            CategoryId.HasValue ||
            MinPrice.HasValue ||
            MaxPrice.HasValue;

        public string ToQueryString(int? overridePage = null)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(Keyword))
                parts.Add($"keyword={Uri.EscapeDataString(Keyword)}");
            if (CategoryId.HasValue)
                parts.Add($"categoryId={CategoryId}");
            if (MinPrice.HasValue)
                parts.Add($"minPrice={MinPrice}");
            if (MaxPrice.HasValue)
                parts.Add($"maxPrice={MaxPrice}");
            if (!string.IsNullOrWhiteSpace(SortBy))
                parts.Add($"sortBy={SortBy}");

            parts.Add($"page={overridePage ?? Page}");
            parts.Add($"pageSize={PageSize}");

            return string.Join("&", parts);
        }
    }
}
