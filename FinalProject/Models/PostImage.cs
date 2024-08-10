namespace FinalProject.Models;

public class PostImage
{
    public int Id { get; set; }
    public string Image { get; set; } = string.Empty;
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;
}
