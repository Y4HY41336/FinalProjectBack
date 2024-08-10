namespace FinalProject.Models;

public class Post
{
    public int Id { get; set; }
    public int RepostCount { get; set; }
    public int LikeCount { get; set; }
    public int BookmarkCount { get; set; }
    public int ReplyCount { get; set; }

    public bool IsActive { get; set; }
    public bool IsQuote { get; set; }
    public bool IsReply { get; set; }

    public string Content { get; set; } = null!;
    public string? Image { get; set; } = string.Empty!;
    public DateTime CreatedDate { get; set; }

    public string UserId { get; set; } = null!;
    public AppUser User { get; set; } = null!;

    public int? CommentedPostId { get; set; }
    public Post? CommentedPost { get; set; }

    public ICollection<Repost>? Reposts { get; set; }
    public ICollection<Like>? Likes { get; set; }
    public ICollection<Bookmark>? Bookmarks { get; set; }
    public ICollection<PostTrend>? PostTrends { get; set; }
    public ICollection<PostImage>? PostImages { get; set; }
    public ICollection<Post>? Comments { get; set; }
    public ICollection<Notification>? Notifications { get; set; }


}
