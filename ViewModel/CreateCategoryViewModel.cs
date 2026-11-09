using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Please select an event.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid event.")]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
