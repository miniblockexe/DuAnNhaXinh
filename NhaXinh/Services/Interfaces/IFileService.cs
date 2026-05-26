namespace NhaXinh.Services.Interfaces
{
    public enum ImageFolder
    {
        Products,
        Banners,
        News
    }
    public interface IFileService
    {
        Task<(bool Success, string Message, string? FilePath)> SaveImageAsync(
           IFormFile file,
           ImageFolder folder
       );

        void DeleteImage(string? relativePath);

        (bool IsValid, string Message) IsValidImage(IFormFile file);
    }
}
