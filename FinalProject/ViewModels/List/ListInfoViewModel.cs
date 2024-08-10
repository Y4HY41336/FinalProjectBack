using FinalProject.Models;

namespace FinalProject.ViewModels.List;

public class ListInfoViewModel
{
    public AppUser CurrentUser { get; set; } = null!;
    public AppUser Owner { get; set; } = null!;
    public ListFollower? Follows { get; set; }
    public UserList List { get; set; } = null!;
}
