using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.AuthViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
