using System.ComponentModel.DataAnnotations;

namespace MyAPP.ViewModels
{
    public class PaymentViewModel
    {
        [Required]
        public string PlanName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public int DurationMonths { get; set; }

        // UPI Payment Details
        [Required(ErrorMessage = "UPI ID is required")]
        [Display(Name = "Your UPI ID")]
        [RegularExpression(@"^[\w.-]+@[\w]+$", ErrorMessage = "Invalid UPI ID format (e.g., name@upi, name@paytm)")]
        public string UpiId { get; set; } = string.Empty;

        // UPI Transaction Reference Number
        [Required(ErrorMessage = "Transaction Reference Number is required")]
        [Display(Name = "UPI Transaction ID / UTR Number")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Enter valid transaction reference (6-50 characters)")]
        public string UtrNumber { get; set; } = string.Empty;

        // Payment method selection
        [Required]
        public string PaymentMethod { get; set; } = "GPay";
    }

    public class PaymentConfirmationViewModel
    {
        public string TransactionId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string UpiId { get; set; } = string.Empty;
        public string UtrNumber { get; set; } = string.Empty;
    }
}
