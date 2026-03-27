using System.ComponentModel.DataAnnotations;

namespace MyAPP.ViewModels
{
    public class AlertViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Train number is required")]
        [StringLength(20, ErrorMessage = "Train number cannot exceed 20 characters")]
        [Display(Name = "Train Number")]
        public string TrainNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Travel date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Travel Date")]
        public DateTime TravelDate { get; set; } = DateTime.Today.AddDays(1);

        [StringLength(100)]
        [Display(Name = "From Station")]
        public string? FromStation { get; set; }

        [StringLength(100)]
        [Display(Name = "To Station")]
        public string? ToStation { get; set; }

        [StringLength(500)]
        [Display(Name = "Message (Optional)")]
        public string? Message { get; set; }

        // For admin broadcast
        public bool IsBroadcast { get; set; } = false;
    }
}
