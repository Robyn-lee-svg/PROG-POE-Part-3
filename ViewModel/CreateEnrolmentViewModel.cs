using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class CreateEnrolmentViewModel
    {
        [Required(ErrorMessage = "Please select an event.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid event.")]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int CategoryID { get; set; }
    }
}
