using System.ComponentModel.DataAnnotations;

namespace Code123.RegistrationModels.ViewModels
{   
    public class LoginViewModel
    {
        [Required (ErrorMessage = "Username or email is required.")]
        [Display (Name = "Username or Email")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}


