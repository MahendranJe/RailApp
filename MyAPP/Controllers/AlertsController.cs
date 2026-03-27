using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAPP.Models;
using MyAPP.Services;
using MyAPP.ViewModels;

namespace MyAPP.Controllers
{
    [Authorize]
    public class AlertsController : Controller
    {
        private readonly IAlertService _alertService;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlertsController(
            IAlertService alertService, 
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager)
        {
            _alertService = alertService;
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

            var alerts = await _alertService.GetUserAlertsAsync(user.Id);
            return View(alerts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AlertViewModel { TravelDate = DateTime.Today.AddDays(1) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlertViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (model.TravelDate < DateTime.Today)
            {
                ModelState.AddModelError("TravelDate", "Travel date cannot be in the past.");
                return View(model);
            }

            await _alertService.CreateAlertAsync(model, user.Id);
            TempData["SuccessMessage"] = "Alert created successfully! You'll be notified when tickets become available.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _alertService.DeleteAlertAsync(id, user.Id);
            if (result)
            {
                TempData["SuccessMessage"] = "Alert deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete alert.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
