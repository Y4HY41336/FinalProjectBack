using FinalProject.Models;

namespace FinalProject.ViewModels.List;

public class ListDiscoverViewModel
{
    public List<UserList>? Lists { get; set; }
    public List<ListFollower>? FollowedLists { get; set; }
    public AppUser? CurrentUser { get; set; }

}
