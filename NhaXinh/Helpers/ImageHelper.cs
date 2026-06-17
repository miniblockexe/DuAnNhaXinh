namespace NhaXinh.Helpers
{
    public static class ImageHelper
    {
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public static string? Validate(IFormFile? file)
        {
            if (file is null || file.Length == 0)
                return "Vui lòng chọn file ảnh.";

            if (file.Length > MaxFileSizeBytes)
                return $"File ảnh không được vượt quá {MaxFileSizeBytes / 1024 / 1024} MB.";

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return $"Chỉ chấp nhận định dạng: {string.Join(", ", AllowedExtensions)}.";

            return null;
        }

        public static bool IsValidImage(IFormFile? file)
            => Validate(file) is null;

        public static string GenerateFileName(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return $"{Guid.NewGuid():N}{ext}";
        }
        public static string WithFallback(string? imageUrl, string defaultPath = "/images/defaults/no-image.png")
            => string.IsNullOrWhiteSpace(imageUrl) ? defaultPath : imageUrl;

        public static void DeleteIfExists(string? imageUrl, string wwwRootPath)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;

            var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(wwwRootPath, relativePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
