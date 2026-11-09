using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class CreateEventViewModel
    {
        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Event date is required.")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000, ErrorMessage = "Distance must be greater than zero.")]
        public decimal Distance { get; set; }

        [Required]
        [RegularExpression("^(Run|Walk|Cycle)$",
            ErrorMessage = "Select Run, Walk or Cycle.")]
        public string EventType { get; set; } = "Run";
    }
}
