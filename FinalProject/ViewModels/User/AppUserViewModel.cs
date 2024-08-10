using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.UserViewModels;

public class AppUserViewModel
{
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    [Required, DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
    
}
