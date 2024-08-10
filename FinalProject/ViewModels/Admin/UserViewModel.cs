using FinalProject.Models;

namespace FinalProject.ViewModels.Admin;

public class UserViewModel
{
    public AppUser User { get; set; } = null!;
    public IList<string> Roles { get; set; } = null!;
}
