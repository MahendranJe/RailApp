using MyAPP.Data;
using MyAPP.Models;
using Microsoft.EntityFrameworkCore;

namespace MyAPP.Services
{
    public class AlertBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AlertBackgroundService> _logger;

        public AlertBackgroundService(IServiceProvider serviceProvider, ILogger<AlertBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Alert Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAlertsAsync();
                    await CheckExpiredSubscriptionsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking alerts.");
                }

                // Check every 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task CheckAlertsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var pendingAlerts = await context.Alerts
                .Include(a => a.User)
                .Where(a => a.IsActive && !a.IsNotified && a.TravelDate >= DateTime.Today)
                .ToListAsync();

            foreach (var alert in pendingAlerts)
            {
                // Mock logic: Randomly determine availability (30% chance of availability)
                var isAvailable = new Random().Next(1, 100) <= 30;

                if (isAvailable)
                {
                    alert.IsAvailable = true;
                    alert.IsNotified = true;
                    alert.NotifiedAt = DateTime.UtcNow;

                    // Create notification
                    var notification = new Notification
                    {
                        UserId = alert.UserId,
                        Title = "?? Ticket Available!",
                        Message = $"Good news! Tickets for train {alert.TrainNumber} on {alert.TravelDate:dd MMM yyyy} " +
                                  $"({alert.FromStation} ? {alert.ToStation}) are now available. Book now!",
                        CreatedAt = DateTime.UtcNow
                    };
                    context.Notifications.Add(notification);

                    _logger.LogInformation($"Alert {alert.Id}: Tickets available for train {alert.TrainNumber}");
                }
            }

            await context.SaveChangesAsync();
        }

        private async Task CheckExpiredSubscriptionsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var expiredSubscriptions = await context.Subscriptions
                .Where(s => s.IsActive && s.ExpiryDate != null && s.ExpiryDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var subscription in expiredSubscriptions)
            {
                subscription.IsActive = false;

                // Create free subscription
                var freeSubscription = new Subscription
                {
                    UserId = subscription.UserId,
                    PlanName = "Free",
                    Price = 0,
                    StartDate = DateTime.UtcNow,
                    IsActive = true
                };
                context.Subscriptions.Add(freeSubscription);

                // Create notification
                var notification = new Notification
                {
                    UserId = subscription.UserId,
                    Title = "Subscription Expired",
                    Message = $"Your {subscription.PlanName} subscription has expired. Renew now to continue accessing premium content.",
                    CreatedAt = DateTime.UtcNow
                };
                context.Notifications.Add(notification);

                _logger.LogInformation($"Subscription {subscription.Id} expired for user {subscription.UserId}");
            }

            await context.SaveChangesAsync();
        }
    }
}
