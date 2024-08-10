namespace FinalProject.Models;

public class Member
{
    public int Id { get; set; }

    public int ListId { get; set; }
    public UserList List { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public AppUser User { get; set; } = null!;
}
