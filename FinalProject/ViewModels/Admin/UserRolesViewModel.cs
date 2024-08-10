using Microsoft.AspNetCore.Identity;

namespace FinalProject.ViewModels.Admin;

public class UserRolesViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public IList<string> UserRoles { get; set; }
    public IList<IdentityRole> AllRoles { get; set; }
    public IList<string> SelectedRoles { get; set; }
    public string? ProfilePhoto { get; set; }
}
