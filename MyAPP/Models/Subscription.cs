using System.ComponentModel.DataAnnotations;

namespace MyAPP.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PlanName { get; set; } = "Free"; // Free, Monthly, Premium

        public decimal Price { get; set; } = 0;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ApplicationUser? User { get; set; }
    }
}
