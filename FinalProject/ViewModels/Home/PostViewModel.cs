using FinalProject.Models;

namespace FinalProject.ViewModels.HomeViewModels
{
    public class PostViewModel
    {
        public List<Post> Posts { get; set; } = null!;
        public Post? MainPost { get; set; }
        public AppUser CurrentUser { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public List<Bookmark> Bookmarks { get; set; } = null!;
        public List<Like> Likes { get; set; } = null!;
        public List<Repost> Reposts { get; set; } = null!;
        public List<Follow>? Follows { get; set; } = null!;
        public List<PostImage>? PostImages { get; set; }
        public int ImageCount { get; set; }
    }
}
