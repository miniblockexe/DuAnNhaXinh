using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        private static readonly Dictionary<ImageFolder, string> FolderMap = new()
        {
            { ImageFolder.Products, "images/products" },
            { ImageFolder.Banners,  "images/banners"  },
            { ImageFolder.News,     "images/news"     }
        };

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<(bool Success, string Message, string? FilePath)> SaveImageAsync(
            IFormFile file,
            ImageFolder folder)
        {
            var (isValid, errorMessage) = IsValidImage(file);
            if (!isValid)
                return (false, errorMessage, null);

            try
            {
                var relativeFolderPath = FolderMap[folder];
                var absoluteFolderPath = Path.Combine(_env.WebRootPath, relativeFolderPath);

                if (!Directory.Exists(absoluteFolderPath))
                    Directory.CreateDirectory(absoluteFolderPath);

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var absoluteFilePath = Path.Combine(absoluteFolderPath, uniqueFileName);

                using var stream = new FileStream(absoluteFilePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var relativePath = $"/{relativeFolderPath}/{uniqueFileName}";
                return (true, "Tải ảnh lên thành công.", relativePath);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi lưu ảnh: {ex.Message}", null);
            }
        }

        public void DeleteImage(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            try
            {
                var absolutePath = Path.Combine(
                    _env.WebRootPath,
                    relativePath.TrimStart('/')
                );

                if (File.Exists(absolutePath))
                    File.Delete(absolutePath);
            }
            catch { }
        }

        public (bool IsValid, string Message) IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "Vui lòng chọn file ảnh.");

            if (file.Length > MaxFileSizeBytes)
                return (false, $"Dung lượng ảnh không được vượt quá {MaxFileSizeBytes / 1024 / 1024} MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return (false, $"Chỉ chấp nhận định dạng: {string.Join(", ", AllowedExtensions)}.");

            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return (false, "Loại file không hợp lệ.");

            return (true, string.Empty);
        }
    }
}
