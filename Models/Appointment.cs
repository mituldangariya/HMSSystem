using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HMSSystem.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [StringLength(50)]
        public string TimeSlot { get; set; }

        [Required]
        [StringLength(500)]
        public string Purpose { get; set; }

        public string? Description { get; set; }

        [StringLength(500)]
        public string? DocumentPath { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string? AdminRemark { get; set; }

        [DataType(DataType.Date)]
        public DateTime? RescheduledDate { get; set; }

        [StringLength(50)]
        public string? RescheduledTime { get; set; }

        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("ApprovedBy")]
        public User AdminUser { get; set; }

        public ICollection<Feedback>? Feedbacks { get; set; }
    }

}
