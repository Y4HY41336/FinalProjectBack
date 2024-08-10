using FinalProject.Models;

namespace FinalProject.ViewModels.Search;

public class SearchViewModel
{
    public List<Post>? Posts { get; set; }
    public List<AppUser>? Users { get; set; }
    public List<UserList>? Lists { get; set; }
    public string? Search { get; set; }
    public string? TabId { get; set; }

}
