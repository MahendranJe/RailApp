using System.ComponentModel.DataAnnotations;

namespace MyAPP.Models
{
    public class TrainUpdate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string TrainNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FromStation { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ToStation { get; set; } = string.Empty;

        // Legacy field - kept for backward compatibility
        public DateTime TravelDate { get; set; }

        // New Schedule Fields
        public ScheduleType ScheduleType { get; set; } = ScheduleType.OneTime;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(500)]
        public string? ImagePath { get; set; }

        public bool IsPremium { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public string? CreatedByUserId { get; set; }

        // Navigation properties
        public virtual ApplicationUser? CreatedBy { get; set; }
        public virtual ICollection<TrainScheduleDay> ScheduleDays { get; set; } = new List<TrainScheduleDay>();
    }
}
