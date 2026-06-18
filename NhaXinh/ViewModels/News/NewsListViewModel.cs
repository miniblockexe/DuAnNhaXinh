namespace NhaXinh.ViewModels.News
{
    public class NewsListViewModel
    {
        public List<NhaXinh.Models.News> NewsList { get; set; } = new();
        public List<NhaXinh.Models.News> FeaturedNews { get; set; } = new();

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public string? SearchKeyword { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
