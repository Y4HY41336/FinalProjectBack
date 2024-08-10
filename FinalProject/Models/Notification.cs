namespace FinalProject.Models;

public class Notification
{
    public int Id { get; set; }

    public bool IsRead { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }

    public string Type { get; set; } = null!;
    public string Message { get; set; } = string.Empty;

    public string UserId { get; set; } = null!;
    public AppUser User { get; set; }

    public string ReceiverId { get; set; } = null!;
    public AppUser Receiver { get; set; }

    public int PostId { get; set; }
    public Post? Post { get; set; }

}
