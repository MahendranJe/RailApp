using MyAPP.Models;
using MyAPP.ViewModels;

namespace MyAPP.Services
{
    public interface ISubscriptionService
    {
        Task<Subscription?> GetUserSubscriptionAsync(string userId);
        Task<Subscription?> GetActiveSubscriptionAsync(string userId);
        Task<bool> HasActiveSubscriptionAsync(string userId);
        Task<bool> CanViewPremiumContentAsync(string userId);
        Task<Subscription> CreateSubscriptionAsync(string userId, string planName, decimal price, int durationMonths);
        Task<bool> CancelSubscriptionAsync(string userId);
        Task<List<Subscription>> GetAllSubscriptionsAsync();
    }
}
