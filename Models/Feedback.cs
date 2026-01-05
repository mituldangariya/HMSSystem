using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HMSSystem.Models
{
    [Table("Feedback")] 
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comments { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("AppointmentId")]
        public Appointment Appointment { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }

}
