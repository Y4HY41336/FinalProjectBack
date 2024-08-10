using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.Profile;

public class UpdateProfileViewModel
{
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Bio { get; set; } = null!;
    [Required]
    public string Location { get; set; } = null!;
    public IFormFile? HeaderPhoto { get; set; }
    public IFormFile? ProfilePhoto { get; set; }
}
