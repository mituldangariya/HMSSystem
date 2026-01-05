using System.ComponentModel.DataAnnotations;

namespace HMSSystem.Models.ViewModels
{
    public class FeedbackViewModel
    {
        public int AppointmentId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comments { get; set; }
    }
}
