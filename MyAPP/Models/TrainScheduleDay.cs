using System.ComponentModel.DataAnnotations;

namespace MyAPP.Models
{
    public class TrainScheduleDay
    {
        [Key]
        public int Id { get; set; }

        public int TrainUpdateId { get; set; }

        [Required]
        [StringLength(10)]
        public string DayOfWeek { get; set; } = string.Empty; // Mon, Tue, Wed, Thu, Fri, Sat, Sun

        // Navigation property
        public virtual TrainUpdate? TrainUpdate { get; set; }
    }
}
