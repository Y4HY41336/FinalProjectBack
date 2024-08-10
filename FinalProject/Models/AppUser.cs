using Microsoft.AspNetCore.Identity;

namespace FinalProject.Models;


public class AppUser : IdentityUser
{
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsPremium { get; set; }
    public bool IsVerified { get; set; }
    public bool IsGovermentVerified { get; set; }

    public string Name { get; set; } = null!;
    public string ProfilePhoto { get; set; } = "default.png";
    public string HeaderPhoto { get; set; } = "header-default.jpg";
    public string Bio { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }

    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }

    public ICollection<Post>? Posts { get; set; }
    public ICollection<Repost>? Reposts { get; set; }
    public ICollection<Like>? Likes { get; set; }
    public ICollection<Bookmark>? Bookmarks { get; set; }
    public ICollection<Follow>? Follower { get; set; }
    public ICollection<Follow>? Followed { get; set; }
    public ICollection<Member>? Members { get; set; }
    public ICollection<ListFollower>? ListFollowers { get; set; }
    public ICollection<Notification>? SentNotifications { get; set; }
    public ICollection<Notification>? ReceivedNotifications { get; set; }

}
