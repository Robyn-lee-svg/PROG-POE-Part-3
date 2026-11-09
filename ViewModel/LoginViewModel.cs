using System.ComponentModel.DataAnnotations;


namespace RaceDay.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your password.")]
        public string Password { get; set; } = string.Empty;
    }
}
