
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Organiser|Participant)$")]
        public string Role { get; set; } = "Participant";

        [Phone]
        public string? Phone { get; set; }
    }
}

