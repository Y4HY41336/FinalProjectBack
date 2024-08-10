namespace FinalProject.Models;

public class Follow
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }

    public string FollowerId { get; set; }
    public AppUser Follower { get; set; }
    public string FollowedId { get; set; }
    public AppUser Followed { get; set; }


}
