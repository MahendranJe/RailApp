using Microsoft.EntityFrameworkCore;
using MyAPP.Data;
using MyAPP.Models;
using MyAPP.ViewModels;

namespace MyAPP.Services
{
    public class AlertService : IAlertService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AlertService> _logger;

        public AlertService(ApplicationDbContext context, ILogger<AlertService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Alert>> GetUserAlertsAsync(string userId)
        {
            return await _context.Alerts
                .Where(a => a.UserId == userId && a.IsActive && !a.IsBroadcast)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Alert?> GetAlertByIdAsync(int id)
        {
            return await _context.Alerts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
        }

        public async Task<Alert> CreateAlertAsync(AlertViewModel model, string userId)
        {
            var alert = new Alert
            {
                UserId = userId,
                TrainNumber = model.TrainNumber,
                TravelDate = model.TravelDate,
                FromStation = model.FromStation,
                ToStation = model.ToStation,
                Message = model.Message,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsBroadcast = false
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();

            // Create notification for alert creation
            var notification = new Notification
            {
                UserId = userId,
                Title = "Alert Created",
                Message = $"Your alert for train {model.TrainNumber} on {model.TravelDate:dd MMM yyyy} has been created.",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return alert;
        }

        public async Task<bool> DeleteAlertAsync(int id, string userId)
        {
            var alert = await _context.Alerts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (alert == null)
                return false;

            alert.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Alert>> GetPendingAlertsAsync()
        {
            return await _context.Alerts
                .Include(a => a.User)
                .Where(a => a.IsActive && !a.IsNotified && !a.IsBroadcast && a.TravelDate >= DateTime.Today)
                .ToListAsync();
        }

        public async Task UpdateAlertStatusAsync(int alertId, bool isAvailable)
        {
            var alert = await _context.Alerts.FindAsync(alertId);
            if (alert != null && !string.IsNullOrEmpty(alert.UserId))
            {
                alert.IsAvailable = isAvailable;
                if (isAvailable)
                {
                    alert.IsNotified = true;
                    alert.NotifiedAt = DateTime.UtcNow;

                    // Create notification
                    var notification = new Notification
                    {
                        UserId = alert.UserId,
                        Title = "?? Ticket Available!",
                        Message = $"Good news! Tickets for train {alert.TrainNumber} on {alert.TravelDate:dd MMM yyyy} are now available. Book now!",
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Notifications.Add(notification);
                }
                await _context.SaveChangesAsync();
            }
        }

        // ==================== ADMIN FUNCTIONS ====================

        public async Task<List<Alert>> GetAllAlertsAsync()
        {
            return await _context.Alerts
                .Include(a => a.User)
                .Include(a => a.CreatedBy)
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetActiveAlertsCountAsync()
        {
            return await _context.Alerts
                .CountAsync(a => a.IsActive && !a.IsBroadcast);
        }

        public async Task<bool> MarkAlertAvailableAsync(int alertId)
        {
            var alert = await _context.Alerts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == alertId && a.IsActive);

            if (alert == null)
                return false;

            alert.IsAvailable = true;
            alert.IsNotified = true;
            alert.NotifiedAt = DateTime.UtcNow;

            // Create notification for the user
            if (!string.IsNullOrEmpty(alert.UserId))
            {
                var notification = new Notification
                {
                    UserId = alert.UserId,
                    Title = "?? Ticket Available!",
                    Message = $"Good news! Tickets for train {alert.TrainNumber} on {alert.TravelDate:dd MMM yyyy} " +
                              $"({alert.FromStation} ? {alert.ToStation}) are now available. Book now!",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Alert {alertId} marked as available by admin");
            return true;
        }

        public async Task<bool> AdminDeleteAlertAsync(int alertId)
        {
            var alert = await _context.Alerts.FindAsync(alertId);
            if (alert == null)
                return false;

            alert.IsActive = false;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Alert {alertId} deleted by admin");
            return true;
        }

        public async Task<Alert> CreateBroadcastAlertAsync(AlertViewModel model, string adminUserId)
        {
            // Create broadcast alert
            var alert = new Alert
            {
                UserId = null, // Broadcast - no specific user
                TrainNumber = model.TrainNumber,
                TravelDate = model.TravelDate,
                FromStation = model.FromStation,
                ToStation = model.ToStation,
                Message = model.Message,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsBroadcast = true,
                IsNotified = true,
                NotifiedAt = DateTime.UtcNow,
                CreatedByUserId = adminUserId
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();

            // Send notification to ALL users
            var allUsers = await _context.Users.Where(u => u.IsActive).ToListAsync();
            var notifications = new List<Notification>();

            foreach (var user in allUsers)
            {
                var notification = new Notification
                {
                    UserId = user.Id,
                    Title = $"?? Train Alert: {model.TrainNumber}",
                    Message = !string.IsNullOrEmpty(model.Message) 
                        ? model.Message 
                        : $"Important update for train {model.TrainNumber} on {model.TravelDate:dd MMM yyyy} " +
                          $"({model.FromStation} ? {model.ToStation}). Check details now!",
                    CreatedAt = DateTime.UtcNow
                };
                notifications.Add(notification);
            }

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Broadcast alert created by admin {adminUserId}, notified {allUsers.Count} users");
            return alert;
        }
    }
}
