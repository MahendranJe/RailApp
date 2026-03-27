using MyAPP.Models;

namespace MyAPP.ViewModels
{
    public class TrainUpdateDetailsViewModel
    {
        public TrainUpdate TrainUpdate { get; set; } = null!;
        public bool CanViewFullDetails { get; set; }
        public bool IsLoggedIn { get; set; }
        public string? CurrentPlan { get; set; }
    }
}
