using System.Text.RegularExpressions;
using System.Text;

namespace NhaXinh.Helpers
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var slug = RemoveDiacritics(input);

            slug = slug.ToLowerInvariant();

            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", " ");

            slug = Regex.Replace(slug, @"\s+", " ").Trim();

            slug = slug.Replace(" ", "-");

            slug = Regex.Replace(slug, @"-+", "-");

            slug = slug.Trim('-');

            return slug;
        }

        public static string GenerateUniqueSlug(string input, IEnumerable<string> existingSlugs)
        {
            var baseSlug = GenerateSlug(input);
            var slug = baseSlug;
            var counter = 1;

            while (existingSlugs.Contains(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        private static string RemoveDiacritics(string input)
        {
            input = input
                .Replace("đ", "d")
                .Replace("Đ", "D");

            var normalized = input.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalized)
            {
                var category = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != System.Globalization.UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
