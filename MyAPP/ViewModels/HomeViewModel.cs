using MyAPP.Models;

namespace MyAPP.ViewModels
{
    public class HomeViewModel
    {
        public List<TrainUpdate> TrainUpdates { get; set; } = new List<TrainUpdate>();
        public string? SearchTerm { get; set; }
        public DateTime? FilterDate { get; set; }
        public string? FilterFromStation { get; set; }
        public string? FilterToStation { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool IsPremiumUser { get; set; }
        public int UnreadNotifications { get; set; }
        public bool IsSubscribed { get; set; }
        public string? CurrentPlan { get; set; }
    }
}
