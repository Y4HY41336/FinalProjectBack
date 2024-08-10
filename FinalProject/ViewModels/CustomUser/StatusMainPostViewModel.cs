using FinalProject.Models;

namespace FinalProject.ViewModels.CustomUser
{
    public class StatusMainPostViewModel
    {
        public List<Post> Replies { get; set; } = null!;
        public Post MainPost { get; set; } = null!;
        public AppUser CurrentUser { get; set; } = null!;
        public AppUser PostOwner { get; set; } = null!;
        public List<Bookmark> Bookmarks { get; set; } = null!;
        public List<Like> Likes { get; set; } = null!;
        public List<Repost> Reposts { get; set; } = null!;
        public List<PostImage>? PostImages { get; set; }
        public List<Follow>? Follows { get; set; }
        public int ImageCount { get; set; }
    }
}
