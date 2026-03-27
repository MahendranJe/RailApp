using System.ComponentModel.DataAnnotations;

namespace MyAPP.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int? SubscriptionId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PlanName { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed

        [StringLength(100)]
        public string? TransactionId { get; set; }

        [StringLength(20)]
        public string PaymentMethod { get; set; } = "UPI"; // UPI, GPay, PhonePe, Paytm

        [StringLength(100)]
        public string? PayerUpiId { get; set; }

        [StringLength(100)]
        public string? MerchantUpiId { get; set; }

        // UTR Number for payment verification
        [StringLength(50)]
        public string? UtrNumber { get; set; }

        // Navigation properties
        public virtual ApplicationUser? User { get; set; }
        public virtual Subscription? Subscription { get; set; }
    }
}
