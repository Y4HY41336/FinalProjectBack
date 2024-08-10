using FinalProject.Models;

namespace FinalProject.ViewModels.List;

public class UpdateListViewModel
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
    public UserList List { get; set; } = null!;
}
