namespace FinalProject.Models;

public class UserList
{
    public int Id { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsDeleted { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string ProfilePhoto { get; set; } = "DefaultList.png";

    public int FollowerCount { get; set; }
    public int MemberCount { get; set; }
    public DateTime CreatedDate { get; set; }

    public string OwnerId { get; set; } = null!;
    public AppUser Owner { get; set; } = null!;

    public ICollection<Member>? Members { get; set; }
    public ICollection<ListFollower>? ListFollowers { get; set; }

}
