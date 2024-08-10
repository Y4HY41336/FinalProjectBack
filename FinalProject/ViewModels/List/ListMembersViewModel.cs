using FinalProject.Models;

namespace FinalProject.ViewModels.List;

public class ListMembersViewModel
{
    public List<AppUser> Users { get; set; } = null!;
    public AppUser CurrentUser { get; set; } = null!;
    public List<Member>? Members { get; set; }
    public List<ListFollower>? Followers { get; set; }
    public List<Follow>? Follow { get; set; }
}
