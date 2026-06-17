using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class FileService : IFileService
    {
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/png", "image/webp"];

        private static readonly Dictionary<ImageFolder, string> FolderMap = new()
        {
            { ImageFolder.Products,   "images/products"   },
            { ImageFolder.Banners,    "images/banners"    },
            { ImageFolder.News,       "images/news"       },
            { ImageFolder.Avatars,    "images/avatars"    },
            { ImageFolder.Categories, "images/categories" }
        };

        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileService> _logger;

        public FileService(IWebHostEnvironment env, ILogger<FileService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, string? FilePath)> SaveImageAsync(
            IFormFile file, ImageFolder folder)
        {
            var (isValid, errorMessage) = IsValidImage(file);
            if (!isValid)
                return (false, errorMessage, null);

            try
            {
                var relativeFolderPath = FolderMap[folder];
                var absoluteFolderPath = Path.Combine(_env.WebRootPath, relativeFolderPath);

                Directory.CreateDirectory(absoluteFolderPath);

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{extension}";
                var absolutePath = Path.Combine(absoluteFolderPath, fileName);

                using var stream = new FileStream(absolutePath, FileMode.Create);
                await file.CopyToAsync(stream);

                return (true, "Tải ảnh lên thành công.", $"/{relativeFolderPath}/{fileName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu ảnh vào folder {Folder}", folder);
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
                    _env.WebRootPath, relativePath.TrimStart('/'));

                if (File.Exists(absolutePath))
                    File.Delete(absolutePath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không xóa được file ảnh: {Path}", relativePath);
            }
        }

        public (bool IsValid, string Message) IsValidImage(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return (false, "Vui lòng chọn file ảnh.");

            if (file.Length > MaxFileSizeBytes)
                return (false, $"Dung lượng ảnh không được vượt quá {MaxFileSizeBytes / 1024 / 1024} MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return (false, $"Chỉ chấp nhận định dạng: {string.Join(", ", AllowedExtensions)}.");

            if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return (false, "Loại file không hợp lệ.");

            return (true, string.Empty);
        }
    }
}