namespace FinalProject.Models;

public class Repost
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public AppUser User { get; set; } = null!;
}
