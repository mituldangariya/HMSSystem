using System.ComponentModel.DataAnnotations;

namespace HMSSystem.Models.ViewModels
{
    public class AppointmentViewModel
    {
        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Display(Name = "Time Slot")]
        public string TimeSlot { get; set; }

        [Required]
        [StringLength(500)]
        public string Purpose { get; set; }

        public string Description { get; set; }

        [Display(Name = "Upload Document (Optional)")]
        public IFormFile Document { get; set; }
    }
}
