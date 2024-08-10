namespace FinalProject.ViewModels.HomeViewModels
{
    public class CreatePostViewModel
    {
        public string Content { get; set; } = null!;
        public string Image { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        public string TrendName { get; set; } = string.Empty;

        public string UserId { get; set; } = null!;
        public int PostId { get; set; }

        public List<IFormFile>? PostImages { get; set; }
    }
}
