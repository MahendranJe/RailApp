namespace MyAPP.ViewModels
{
    public class SubscriptionViewModel
    {
        public int Id { get; set; }
        public string CurrentPlan { get; set; } = "Free";
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
        public int DaysRemaining => ExpiryDate.HasValue ? Math.Max(0, (ExpiryDate.Value - DateTime.UtcNow).Days) : 0;
    }

    public class SubscriptionPlanViewModel
    {
        public string PlanName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMonths { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new List<string>();
        public bool IsPopular { get; set; }
    }
}
