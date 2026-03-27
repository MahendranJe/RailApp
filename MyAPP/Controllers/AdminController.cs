using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPP.Data;
using MyAPP.Models;
using MyAPP.Services;
using MyAPP.ViewModels;

namespace MyAPP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ITrainUpdateService _trainUpdateService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;
        private readonly IAlertService _alertService;
        private readonly IScheduleService _scheduleService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(
            ITrainUpdateService trainUpdateService,
            ISubscriptionService subscriptionService,
            IPaymentService paymentService,
            INotificationService notificationService,
            IAlertService alertService,
            IScheduleService scheduleService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _trainUpdateService = trainUpdateService;
            _subscriptionService = subscriptionService;
            _paymentService = paymentService;
            _notificationService = notificationService;
            _alertService = alertService;
            _scheduleService = scheduleService;
            _userManager = userManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Dashboard()
        {
            var pendingPayments = await _context.Payments.CountAsync(p => p.Status == "Pending");
            var pendingAlerts = await _context.Alerts.CountAsync(a => a.IsActive && !a.IsNotified && !a.IsBroadcast);
            
            var viewModel = new DashboardViewModel
            {
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalTrainUpdates = await _trainUpdateService.GetTotalCountAsync(),
                TotalAlerts = await _alertService.GetActiveAlertsCountAsync(),
                PendingAlerts = pendingAlerts,
                ActiveSubscriptions = await _context.Subscriptions.CountAsync(s => s.IsActive && s.PlanName != "Free"),
                PremiumUsers = await _context.Subscriptions.CountAsync(s => s.IsActive && (s.PlanName == "Monthly" || s.PlanName == "Premium")),
                TotalPayments = await _context.Payments.CountAsync(p => p.Status == "Completed"),
                TotalRevenue = await _paymentService.GetTotalRevenueAsync(),
                RecentTrainUpdates = await _trainUpdateService.GetAllUpdatesAsync(page: 1, pageSize: 5),
                RecentUsers = await _userManager.Users.OrderByDescending(u => u.CreatedAt).Take(5).ToListAsync(),
                RecentPayments = await _context.Payments.Include(p => p.User).OrderByDescending(p => p.PaymentDate).Take(5).ToListAsync(),
                RecentAlerts = await _context.Alerts.Include(a => a.User).Where(a => a.IsActive && !a.IsBroadcast).OrderByDescending(a => a.CreatedAt).Take(5).ToListAsync()
            };

            ViewBag.PendingPayments = pendingPayments;
            ViewBag.PendingAlerts = pendingAlerts;

            return View(viewModel);
        }

        #region Users Management

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var userListWithRoles = new List<(ApplicationUser User, IList<string> Roles, Subscription? Subscription)>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var subscription = await _subscriptionService.GetActiveSubscriptionAsync(user.Id);
                userListWithRoles.Add((user, roles, subscription));
            }

            return View(userListWithRoles);
        }

        #endregion

        #region Subscriptions Management

        public async Task<IActionResult> Subscriptions()
        {
            var subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            return View(subscriptions);
        }

        #endregion

        #region Payments Management

        public async Task<IActionResult> Payments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return View(payments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApprovePayment(int id)
        {
            var payment = await _context.Payments.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null)
            {
                TempData["ErrorMessage"] = "Payment not found.";
                return RedirectToAction(nameof(Payments));
            }

            if (payment.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Payment is not pending.";
                return RedirectToAction(nameof(Payments));
            }

            // Determine duration based on plan
            int durationMonths = payment.PlanName == "Premium" ? 12 : 1;

            // Create subscription for user
            var subscription = await _subscriptionService.CreateSubscriptionAsync(
                payment.UserId,
                payment.PlanName,
                payment.Amount,
                durationMonths);

            // Update payment
            payment.Status = "Completed";
            payment.SubscriptionId = subscription.Id;
            await _context.SaveChangesAsync();

            // Notify user
            await _notificationService.CreateNotificationAsync(
                payment.UserId,
                "Payment Approved!",
                $"Your payment of Rs.{payment.Amount} has been verified. Your {payment.PlanName} subscription is now active until {subscription.ExpiryDate:dd MMM yyyy}. Enjoy premium access!");

            TempData["SuccessMessage"] = $"Payment approved! {payment.User?.FullName}'s subscription activated.";
            return RedirectToAction(nameof(Payments));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPayment(int id)
        {
            var payment = await _context.Payments.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null)
            {
                TempData["ErrorMessage"] = "Payment not found.";
                return RedirectToAction(nameof(Payments));
            }

            if (payment.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Payment is not pending.";
                return RedirectToAction(nameof(Payments));
            }

            // Update payment status
            payment.Status = "Failed";
            await _context.SaveChangesAsync();

            // Notify user
            await _notificationService.CreateNotificationAsync(
                payment.UserId,
                "Payment Not Verified",
                $"Your payment of Rs.{payment.Amount} for {payment.PlanName} plan could not be verified. Please contact support or try again with correct details.");

            TempData["SuccessMessage"] = $"Payment rejected. User has been notified.";
            return RedirectToAction(nameof(Payments));
        }

        #endregion

        #region Alerts Management

        public async Task<IActionResult> Alerts()
        {
            var alerts = await _alertService.GetAllAlertsAsync();
            return View(alerts);
        }

        [HttpGet]
        public IActionResult CreateBroadcastAlert()
        {
            return View(new AlertViewModel 
            { 
                TravelDate = DateTime.Today.AddDays(1),
                IsBroadcast = true 
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBroadcastAlert(AlertViewModel model)
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

            await _alertService.CreateBroadcastAlertAsync(model, user.Id);
            
            var userCount = await _userManager.Users.CountAsync(u => u.IsActive);
            TempData["SuccessMessage"] = $"Broadcast alert created! {userCount} users have been notified.";
            return RedirectToAction(nameof(Alerts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAlertAvailable(int id)
        {
            var result = await _alertService.MarkAlertAvailableAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Alert marked as available! User has been notified.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update alert.";
            }
            return RedirectToAction(nameof(Alerts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAlert(int id)
        {
            var result = await _alertService.AdminDeleteAlertAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Alert deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete alert.";
            }
            return RedirectToAction(nameof(Alerts));
        }

        #endregion

        #region Train Updates Management

        public async Task<IActionResult> TrainUpdates()
        {
            var updates = await _trainUpdateService.GetAllUpdatesAsync(pageSize: 100);
            
            // Add schedule display info to ViewBag
            var scheduleInfo = new Dictionary<int, (string DisplayText, string BadgeClass, string BadgeText, string? NextRun)>();
            foreach (var update in updates)
            {
                var nextRun = _scheduleService.GetNextRunDate(update);
                scheduleInfo[update.Id] = (
                    _scheduleService.GetScheduleDisplayText(update),
                    _scheduleService.GetScheduleBadgeClass(update.ScheduleType),
                    _scheduleService.GetScheduleBadgeText(update.ScheduleType),
                    nextRun?.ToString("MMM dd, yyyy")
                );
            }
            ViewBag.ScheduleInfo = scheduleInfo;
            ViewBag.DayNames = _scheduleService.GetDayNames();
            
            return View(updates);
        }

        [HttpGet]
        public IActionResult CreateTrainUpdate()
        {
            ViewBag.DayNames = _scheduleService.GetDayNames();
            return View(new TrainUpdateViewModel 
            { 
                TravelDate = DateTime.Today,
                StartDate = DateTime.Today,
                ScheduleType = ScheduleType.OneTime
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrainUpdate(TrainUpdateViewModel model)
        {
            try
            {
                // Validate based on schedule type
                if (!ValidateScheduleModel(model))
                {
                    ViewBag.DayNames = _scheduleService.GetDayNames();
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.DayNames = _scheduleService.GetDayNames();
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                string? imagePath = null;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Validate file size (max 5MB)
                    if (model.ImageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ImageFile", "Image size cannot exceed 5MB");
                        ViewBag.DayNames = _scheduleService.GetDayNames();
                        return View(model);
                    }

                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ImageFile", "Only image files (.jpg, .jpeg, .png, .gif) are allowed");
                        ViewBag.DayNames = _scheduleService.GetDayNames();
                        return View(model);
                    }

                    imagePath = await SaveImageAsync(model.ImageFile);
                }

                await _trainUpdateService.CreateUpdateAsync(model, user.Id, imagePath);
                TempData["SuccessMessage"] = "Train update created successfully!";
                return RedirectToAction(nameof(TrainUpdates));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating train update: {ex.Message}";
                ViewBag.DayNames = _scheduleService.GetDayNames();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditTrainUpdate(int id)
        {
            var update = await _trainUpdateService.GetUpdateByIdAsync(id);
            if (update == null)
            {
                return NotFound();
            }

            var viewModel = new TrainUpdateViewModel
            {
                Id = update.Id,
                Title = update.Title,
                Description = update.Description,
                TrainNumber = update.TrainNumber,
                FromStation = update.FromStation,
                ToStation = update.ToStation,
                TravelDate = update.TravelDate,
                ScheduleType = update.ScheduleType,
                StartDate = update.StartDate,
                EndDate = update.EndDate,
                IsPremium = update.IsPremium,
                ExistingImagePath = update.ImagePath,
                SelectedDay = update.ScheduleDays?.FirstOrDefault()?.DayOfWeek,
                SelectedDays = update.ScheduleDays?.Select(d => d.DayOfWeek).ToList() ?? new List<string>()
            };

            ViewBag.DayNames = _scheduleService.GetDayNames();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainUpdate(TrainUpdateViewModel model)
        {
            try
            {
                // Validate based on schedule type
                if (!ValidateScheduleModel(model))
                {
                    ViewBag.DayNames = _scheduleService.GetDayNames();
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.DayNames = _scheduleService.GetDayNames();
                    return View(model);
                }

                string? imagePath = null;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Validate file size (max 5MB)
                    if (model.ImageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ImageFile", "Image size cannot exceed 5MB");
                        ViewBag.DayNames = _scheduleService.GetDayNames();
                        return View(model);
                    }

                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ImageFile", "Only image files (.jpg, .jpeg, .png, .gif) are allowed");
                        ViewBag.DayNames = _scheduleService.GetDayNames();
                        return View(model);
                    }

                    imagePath = await SaveImageAsync(model.ImageFile);
                }

                var result = await _trainUpdateService.UpdateAsync(model, imagePath);
                if (!result)
                {
                    TempData["ErrorMessage"] = "Failed to update train update.";
                    ViewBag.DayNames = _scheduleService.GetDayNames();
                    return View(model);
                }

                TempData["SuccessMessage"] = "Train update modified successfully!";
                return RedirectToAction(nameof(TrainUpdates));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating train update: {ex.Message}";
                ViewBag.DayNames = _scheduleService.GetDayNames();
                return View(model);
            }
        }

        private bool ValidateScheduleModel(TrainUpdateViewModel model)
        {
            bool isValid = true;

            // Days are required for ALL schedule types
            if (model.SelectedDays == null || !model.SelectedDays.Any())
            {
                ModelState.AddModelError("SelectedDays", "Please select at least one operating day.");
                isValid = false;
            }

            switch (model.ScheduleType)
            {
                case ScheduleType.OneTime:
                    if (!model.StartDate.HasValue)
                    {
                        ModelState.AddModelError("StartDate", "Date is required.");
                        isValid = false;
                    }
                    // For one-time, only one day should be selected
                    if (model.SelectedDays?.Count > 1)
                    {
                        ModelState.AddModelError("SelectedDays", "For one-time schedule, select only one day.");
                        isValid = false;
                    }
                    break;

                case ScheduleType.Daily:
                    if (!model.StartDate.HasValue)
                    {
                        ModelState.AddModelError("StartDate", "Start date is required.");
                        isValid = false;
                    }
                    break;

                case ScheduleType.Weekly:
                    if (!model.StartDate.HasValue)
                    {
                        ModelState.AddModelError("StartDate", "Effective from date is required.");
                        isValid = false;
                    }
                    // For weekly, only one day should be selected
                    if (model.SelectedDays?.Count > 1)
                    {
                        ModelState.AddModelError("SelectedDays", "For weekly schedule, select only one day.");
                        isValid = false;
                    }
                    break;

                case ScheduleType.CustomDays:
                    if (!model.StartDate.HasValue)
                    {
                        ModelState.AddModelError("StartDate", "Effective from date is required.");
                        isValid = false;
                    }
                    break;

                case ScheduleType.DateRange:
                    if (!model.StartDate.HasValue)
                    {
                        ModelState.AddModelError("StartDate", "Start date is required for date range.");
                        isValid = false;
                    }
                    if (!model.EndDate.HasValue)
                    {
                        ModelState.AddModelError("EndDate", "End date is required for date range.");
                        isValid = false;
                    }
                    if (model.StartDate.HasValue && model.EndDate.HasValue && model.EndDate < model.StartDate)
                    {
                        ModelState.AddModelError("EndDate", "End date must be after start date.");
                        isValid = false;
                    }
                    break;
            }

            return isValid;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrainUpdate(int id)
        {
            var result = await _trainUpdateService.DeleteAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Train update deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete train update.";
            }

            return RedirectToAction(nameof(TrainUpdates));
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            try
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "timetables");
                
                // Ensure directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Create unique filename
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Return relative path for database
                return $"/images/timetables/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save image: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
