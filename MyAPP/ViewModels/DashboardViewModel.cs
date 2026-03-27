using MyAPP.Models;

namespace MyAPP.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalTrainUpdates { get; set; }
        public int TotalAlerts { get; set; }
        public int PendingAlerts { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int PremiumUsers { get; set; }
        public int TotalPayments { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<TrainUpdate> RecentTrainUpdates { get; set; } = new List<TrainUpdate>();
        public List<ApplicationUser> RecentUsers { get; set; } = new List<ApplicationUser>();
        public List<Payment> RecentPayments { get; set; } = new List<Payment>();
        public List<Subscription> ActiveSubscriptionsList { get; set; } = new List<Subscription>();
        public List<Alert> RecentAlerts { get; set; } = new List<Alert>();
    }
}
