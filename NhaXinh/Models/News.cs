namespace NhaXinh.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;  
        public string? Summary { get; set; }               
        public string Content { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? Author { get; set; }
        public bool IsFeatured { get; set; } = false;     
        public bool IsPublished { get; set; } = false;      
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}