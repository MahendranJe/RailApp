using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MyAPP.Models;

namespace MyAPP.ViewModels
{
    public class TrainUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Train number is required")]
        [StringLength(20, ErrorMessage = "Train number cannot exceed 20 characters")]
        [Display(Name = "Train Number")]
        public string TrainNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "From station is required")]
        [StringLength(100, ErrorMessage = "Station name cannot exceed 100 characters")]
        [Display(Name = "From Station")]
        public string FromStation { get; set; } = string.Empty;

        [Required(ErrorMessage = "To station is required")]
        [StringLength(100, ErrorMessage = "Station name cannot exceed 100 characters")]
        [Display(Name = "To Station")]
        public string ToStation { get; set; } = string.Empty;

        // Legacy field - kept for backward compatibility
        [DataType(DataType.Date)]
        [Display(Name = "Travel Date")]
        public DateTime TravelDate { get; set; } = DateTime.Today;

        // Schedule Fields
        [Display(Name = "Schedule Type")]
        public ScheduleType ScheduleType { get; set; } = ScheduleType.OneTime;

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        // For Weekly - single day selection
        [Display(Name = "Day of Week")]
        public string? SelectedDay { get; set; }

        // For CustomDays - multiple day selection
        [Display(Name = "Days")]
        public List<string> SelectedDays { get; set; } = new List<string>();

        [Display(Name = "Timetable Image")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImagePath { get; set; }

        [Display(Name = "Premium Content")]
        public bool IsPremium { get; set; } = false;

        // Helper properties for display
        public string? ScheduleDisplayText { get; set; }
        public string? NextRunDate { get; set; }
        public string? ScheduleBadgeClass { get; set; }
        public string? ScheduleBadgeText { get; set; }
    }
}
