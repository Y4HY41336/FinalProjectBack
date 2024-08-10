namespace FinalProject.ViewModels.List;

public class CreateListViewModel
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
    public IFormFile? ProfilePhoto { get; set; }
    public string? Search { get; set; }
}
