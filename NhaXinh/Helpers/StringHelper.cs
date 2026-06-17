namespace NhaXinh.Helpers
{
    public static class StringHelper
    {
        public static string Truncate(string? text, int maxLength = 100)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            return text.Length <= maxLength ? text : text[..maxLength].TrimEnd() + "...";
        }

        public static string StripHtml(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
        }

        public static string Normalize(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            return System.Text.RegularExpressions.Regex.Replace(text.Trim(), @"\s+", " ");
        }

        public static string ToTitleCase(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var info = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            return info.ToTitleCase(text.ToLower());
        }
        public static bool IsPositiveInteger(string? text)
            => int.TryParse(text, out int val) && val > 0;

        public static string Summarize(string? html, int maxLength = 150)
            => Truncate(StripHtml(html), maxLength);
    }
}
