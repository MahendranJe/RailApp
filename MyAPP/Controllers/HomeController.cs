using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyAPP.Models;
using MyAPP.Services;
using MyAPP.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace MyAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITrainUpdateService _trainUpdateService;
        private readonly INotificationService _notificationService;
        private readonly IScheduleService _scheduleService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ITrainUpdateService trainUpdateService,
            INotificationService notificationService,
            IScheduleService scheduleService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _trainUpdateService = trainUpdateService;
            _notificationService = notificationService;
            _scheduleService = scheduleService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search, DateTime? date, string? from, string? to, int page = 1)
        {
            var pageSize = 10;
            var trainUpdates = await _trainUpdateService.GetAllUpdatesAsync(search, date, from, to, page, pageSize);
            var totalCount = await _trainUpdateService.GetTotalCountAsync(search, date, from, to);

            // Build schedule info for each train update
            var scheduleInfo = new Dictionary<int, (string DisplayText, string BadgeClass, string BadgeText, string? NextRun, bool IsRunningToday)>();
            foreach (var update in trainUpdates)
            {
                var nextRun = _scheduleService.GetNextRunDate(update);
                scheduleInfo[update.Id] = (
                    _scheduleService.GetScheduleDisplayText(update),
                    _scheduleService.GetScheduleBadgeClass(update.ScheduleType),
                    _scheduleService.GetScheduleBadgeText(update.ScheduleType),
                    nextRun?.ToString("MMM dd"),
                    _scheduleService.IsRunningToday(update)
                );
            }
            ViewBag.ScheduleInfo = scheduleInfo;

            var viewModel = new HomeViewModel
            {
                TrainUpdates = trainUpdates,
                SearchTerm = search,
                FilterDate = date,
                FilterFromStation = from,
                FilterToStation = to,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };

            // Check if user is logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    viewModel.UnreadNotifications = await _notificationService.GetUnreadCountAsync(user.Id);
                    viewModel.CurrentPlan = "Full Access";
                    viewModel.IsSubscribed = true;
                }
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var trainUpdate = await _trainUpdateService.GetUpdateByIdAsync(id);
            if (trainUpdate == null)
            {
                return NotFound();
            }

            // Add schedule info
            ViewBag.ScheduleDisplayText = _scheduleService.GetScheduleDisplayText(trainUpdate);
            ViewBag.ScheduleBadgeClass = _scheduleService.GetScheduleBadgeClass(trainUpdate.ScheduleType);
            ViewBag.ScheduleBadgeText = _scheduleService.GetScheduleBadgeText(trainUpdate.ScheduleType);
            var nextRun = _scheduleService.GetNextRunDate(trainUpdate);
            ViewBag.NextRunDate = nextRun?.ToString("MMMM dd, yyyy");
            ViewBag.IsRunningToday = _scheduleService.IsRunningToday(trainUpdate);

            var viewModel = new TrainUpdateDetailsViewModel
            {
                TrainUpdate = trainUpdate,
                IsLoggedIn = User.Identity?.IsAuthenticated == true
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    viewModel.CanViewFullDetails = true;
                    viewModel.CurrentPlan = "Full Access";
                }
            }
            else
            {
                viewModel.CanViewFullDetails = false;
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
