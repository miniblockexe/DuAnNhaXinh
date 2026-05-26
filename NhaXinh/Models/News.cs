namespace NhaXinh.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;   // URL thân thiện, unique
        public string? Summary { get; set; }                // Tóm tắt ngắn hiện ở danh sách
        public string Content { get; set; } = string.Empty; // Nội dung HTML đầy đủ
        public string? ThumbnailUrl { get; set; }
        public string? Author { get; set; }
        public bool IsFeatured { get; set; } = false;       // Nổi bật trang chủ
        public bool IsPublished { get; set; } = false;      // Công khai / ẩn bài
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}