using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class EditCategoryViewModel
    {
        public int CategoryID { get; set; }

        public int EventID { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
