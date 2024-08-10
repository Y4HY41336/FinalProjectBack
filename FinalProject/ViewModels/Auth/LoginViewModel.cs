using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.AuthViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UsernameOrEmail { get; set; } = null!;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
