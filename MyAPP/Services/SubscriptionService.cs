using Microsoft.EntityFrameworkCore;
using MyAPP.Data;
using MyAPP.Models;

namespace MyAPP.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Subscription?> GetUserSubscriptionAsync(string userId)
        {
            return await _context.Subscriptions
                .Include(s => s.User)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Subscription?> GetActiveSubscriptionAsync(string userId)
        {
            return await _context.Subscriptions
                .Include(s => s.User)
                .Where(s => s.UserId == userId && s.IsActive)
                .Where(s => s.ExpiryDate == null || s.ExpiryDate > DateTime.UtcNow)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasActiveSubscriptionAsync(string userId)
        {
            var subscription = await GetActiveSubscriptionAsync(userId);
            return subscription != null && subscription.PlanName != "Free";
        }

        public async Task<bool> CanViewPremiumContentAsync(string userId)
        {
            var subscription = await GetActiveSubscriptionAsync(userId);
            if (subscription == null) return false;
            
            return subscription.PlanName == "Monthly" || subscription.PlanName == "Premium";
        }

        public async Task<Subscription> CreateSubscriptionAsync(string userId, string planName, decimal price, int durationMonths)
        {
            // Deactivate existing subscriptions
            var existingSubscriptions = await _context.Subscriptions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();

            foreach (var sub in existingSubscriptions)
            {
                sub.IsActive = false;
            }

            var subscription = new Subscription
            {
                UserId = userId,
                PlanName = planName,
                Price = price,
                StartDate = DateTime.UtcNow,
                ExpiryDate = durationMonths > 0 ? DateTime.UtcNow.AddMonths(durationMonths) : null,
                IsActive = true
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            // Create notification
            var notification = new Notification
            {
                UserId = userId,
                Title = "Subscription Activated!",
                Message = $"Your {planName} subscription is now active. Enjoy premium train updates!",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return subscription;
        }

        public async Task<bool> CancelSubscriptionAsync(string userId)
        {
            var subscription = await GetActiveSubscriptionAsync(userId);
            if (subscription == null || subscription.PlanName == "Free")
                return false;

            subscription.IsActive = false;

            // Create free subscription
            var freeSubscription = new Subscription
            {
                UserId = userId,
                PlanName = "Free",
                Price = 0,
                StartDate = DateTime.UtcNow,
                IsActive = true
            };
            _context.Subscriptions.Add(freeSubscription);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Subscription>> GetAllSubscriptionsAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.User)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();
        }
    }
}
