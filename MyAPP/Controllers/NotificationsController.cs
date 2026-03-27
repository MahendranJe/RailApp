using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAPP.Models;
using MyAPP.Services;

namespace MyAPP.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsController(
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id, 50);
            
            // Mark all as read when viewing the page
            await _notificationService.MarkAllAsReadAsync(user.Id);
            
            return View(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _notificationService.MarkAllAsReadAsync(user.Id);
            TempData["SuccessMessage"] = "All notifications marked as read.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { count = 0 });
            }

            var count = await _notificationService.GetUnreadCountAsync(user.Id);
            return Json(new { count });
        }

        // This endpoint is called by the navbar dropdown
        [HttpGet]
        public async Task<IActionResult> GetRecent()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { unreadCount = 0, notifications = Array.Empty<object>() });
            }

            var unreadCount = await _notificationService.GetUnreadCountAsync(user.Id);
            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id, 5);
            
            return Json(new
            {
                unreadCount = unreadCount,
                notifications = notifications.Select(n => new
                {
                    id = n.Id,
                    title = n.Title,
                    message = n.Message,
                    isRead = n.IsRead,
                    createdAt = n.CreatedAt.ToString("MMM dd, HH:mm")
                })
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetLatest()
        {
            return await GetRecent();
        }
    }
}
