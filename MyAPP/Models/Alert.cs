using System.ComponentModel.DataAnnotations;

namespace MyAPP.Models
{
    public class Alert
    {
        [Key]
        public int Id { get; set; }

        // UserId is null for broadcast alerts (sent to all users)
        public string? UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string TrainNumber { get; set; } = string.Empty;

        [Required]
        public DateTime TravelDate { get; set; }

        [StringLength(100)]
        public string? FromStation { get; set; }

        [StringLength(100)]
        public string? ToStation { get; set; }

        [StringLength(500)]
        public string? Message { get; set; }

        public bool IsNotified { get; set; } = false;

        public bool IsAvailable { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? NotifiedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Indicates if this is a broadcast alert from admin
        public bool IsBroadcast { get; set; } = false;

        // Who created this alert (for admin broadcasts)
        public string? CreatedByUserId { get; set; }

        // Navigation properties
        public virtual ApplicationUser? User { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }
    }
}
