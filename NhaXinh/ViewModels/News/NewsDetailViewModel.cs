namespace NhaXinh.ViewModels.News
{
    public class NewsDetailViewModel
    {
        public NhaXinh.Models.News News { get; set; } = null!;
        public List<NhaXinh.Models.News> RelatedNews { get; set; } = new();
    }
}
