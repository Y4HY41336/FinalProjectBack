using FinalProject.Models;

namespace FinalProject.ViewModels.Profile;

public class FollowsViewModel
{
    public List<AppUser> Following { get; set; }
    public List<AppUser> Followers { get; set; }
}
