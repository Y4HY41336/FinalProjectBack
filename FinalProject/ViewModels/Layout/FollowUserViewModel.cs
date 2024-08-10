using FinalProject.Models;

namespace FinalProject.ViewModels.LayoutViewModels;

public class FollowUserViewModel
{
    public List<AppUser> Users { get; set; } = null!;
    public AppUser CurrentUser { get; set; } = null!;
    public List<Follow>? Follows { get; set; }
    public string? TabId { get; set; }
}
