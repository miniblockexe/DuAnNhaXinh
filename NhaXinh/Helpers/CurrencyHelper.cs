using System.Globalization;

namespace NhaXinh.Helpers
{
    public static class CurrencyHelper
    {
        private static readonly CultureInfo VnCulture = new("vi-VN");

        public static string FormatVnd(decimal amount)
            => amount.ToString("C0", VnCulture);

        public static string FormatNumber(decimal amount)
            => amount.ToString("N0", VnCulture);
        public static int DiscountPercent(decimal originalPrice, decimal discountPrice)
        {
            if (originalPrice <= 0) return 0;
            return (int)Math.Round((originalPrice - discountPrice) / originalPrice * 100);
        }

        public static string DiscountLabel(decimal originalPrice, decimal discountPrice)
            => $"-{DiscountPercent(originalPrice, discountPrice)}%";
    }
}
