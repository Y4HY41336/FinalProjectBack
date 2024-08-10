namespace FinalProject.Models;

public class Bookmark
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string UserId { get; set; }
    public AppUser User { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; }
}
