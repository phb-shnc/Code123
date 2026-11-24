using System.ComponentModel.DataAnnotations;

namespace Code123.RegistrationModels.ViewModels
{
    public class SignupViewModel
    {
        [Required(ErrorMessage = "Invalid Email or Password")]
        [EmailAddress(ErrorMessage = "Invalid Email or Password")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Invalid Email or Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Invalid Email or Password (passwords needs atleast 6 characters).")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Invalid Email or Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage = "Invalid Email or Password (password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }


    
}