using FinalProject.Models;
using FinalProject.ViewModels.Search;

namespace FinalProject.ViewModels.ProfileViewModels;

public class ProfileInfoViewModel
{
    public AppUser? CurrentUser { get; set; }
    public List<Post> Posts { get; set; }
    public Post? MainPost { get; set; }
    public List<Follow>? Follows { get; set; }
}
