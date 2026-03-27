/* 
 * PAYMENT FEATURE - COMMENTED FOR FUTURE USE
 * This entire controller handles subscription and payment functionality.
 * Uncomment this file when enabling premium features.
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAPP.Models;
using MyAPP.Services;
using MyAPP.ViewModels;

namespace MyAPP.Controllers
{
    // [Authorize]
    // public class SubscriptionController : Controller
    // {
    //     private readonly ISubscriptionService _subscriptionService;
    //     private readonly IPaymentService _paymentService;
    //     private readonly UserManager<ApplicationUser> _userManager;

    //     public SubscriptionController(
    //         ISubscriptionService subscriptionService,
    //         IPaymentService paymentService,
    //         UserManager<ApplicationUser> userManager)
    //     {
    //         _subscriptionService = subscriptionService;
    //         _paymentService = paymentService;
    //         _userManager = userManager;
    //     }

    //     public async Task<IActionResult> Index()
    //     {
    //         var user = await _userManager.GetUserAsync(User);
    //         if (user == null)
    //         {
    //             return RedirectToAction("Login", "Account");
    //         }

    //         var subscription = await _subscriptionService.GetActiveSubscriptionAsync(user.Id);

    //         var viewModel = new SubscriptionViewModel
    //         {
    //             Id = subscription?.Id ?? 0,
    //             CurrentPlan = subscription?.PlanName ?? "Free",
    //             StartDate = subscription?.StartDate,
    //             ExpiryDate = subscription?.ExpiryDate,
    //             IsActive = subscription?.IsActive ?? false
    //         };

    //         // Check for pending payments
    //         var pendingPayments = await _paymentService.GetUserPaymentsAsync(user.Id);
    //         ViewBag.HasPendingPayment = pendingPayments.Any(p => p.Status == "Pending");
    //         ViewBag.Plans = GetSubscriptionPlans();

    //         return View(viewModel);
    //     }

    //     [HttpGet]
    //     public IActionResult Subscribe(string plan)
    //     {
    //         var plans = GetSubscriptionPlans();
    //         var selectedPlan = plans.FirstOrDefault(p => p.PlanName == plan);

    //         if (selectedPlan == null || plan == "Free")
    //         {
    //             return RedirectToAction(nameof(Index));
    //         }

    //         var paymentModel = new PaymentViewModel
    //         {
    //             PlanName = selectedPlan.PlanName,
    //             Amount = selectedPlan.Price,
    //             DurationMonths = selectedPlan.DurationMonths,
    //             PaymentMethod = "UPI"
    //         };

    //         ViewBag.MerchantUpiId = _paymentService.GetMerchantUpiId();
    //         ViewBag.MerchantName = _paymentService.GetMerchantName();

    //         return View(paymentModel);
    //     }

    //     [HttpPost]
    //     [ValidateAntiForgeryToken]
    //     public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
    //     {
    //         if (!ModelState.IsValid)
    //         {
    //             ViewBag.MerchantUpiId = _paymentService.GetMerchantUpiId();
    //             ViewBag.MerchantName = _paymentService.GetMerchantName();
    //             return View("Subscribe", model);
    //         }

    //         var user = await _userManager.GetUserAsync(User);
    //         if (user == null)
    //         {
    //             return RedirectToAction("Login", "Account");
    //         }

    //         // Create UPI payment (Status = Pending)
    //         var payment = await _paymentService.CreateUpiPaymentAsync(
    //             user.Id,
    //             model.PlanName,
    //             model.Amount,
    //             model.UpiId,
    //             model.PaymentMethod,
    //             null); // No subscription yet - will be created when admin approves

    //         var confirmationModel = new PaymentConfirmationViewModel
    //         {
    //             TransactionId = payment.TransactionId ?? "",
    //             PlanName = model.PlanName,
    //             Amount = model.Amount,
    //             PaymentDate = payment.PaymentDate,
    //             ExpiryDate = DateTime.UtcNow.AddMonths(model.DurationMonths),
    //             Status = "Pending Verification",
    //             PaymentMethod = model.PaymentMethod,
    //             UpiId = model.UpiId
    //         };

    //         return View("PaymentPending", confirmationModel);
    //     }

    //     [HttpPost]
    //     [ValidateAntiForgeryToken]
    //     public async Task<IActionResult> CancelSubscription()
    //     {
    //         var user = await _userManager.GetUserAsync(User);
    //         if (user == null)
    //         {
    //             return RedirectToAction("Login", "Account");
    //         }

    //         var result = await _subscriptionService.CancelSubscriptionAsync(user.Id);
    //         if (result)
    //         {
    //             TempData["SuccessMessage"] = "Your subscription has been cancelled. You've been moved to the Free plan.";
    //         }
    //         else
    //         {
    //             TempData["ErrorMessage"] = "Failed to cancel subscription.";
    //         }

    //         return RedirectToAction(nameof(Index));
    //     }

    //     public async Task<IActionResult> History()
    //     {
    //         var user = await _userManager.GetUserAsync(User);
    //         if (user == null)
    //         {
    //             return RedirectToAction("Login", "Account");
    //         }

    //         var payments = await _paymentService.GetUserPaymentsAsync(user.Id);
    //         return View(payments);
    //     }

    //     private List<SubscriptionPlanViewModel> GetSubscriptionPlans()
    //     {
    //         return new List<SubscriptionPlanViewModel>
    //         {
    //             new SubscriptionPlanViewModel
    //             {
    //                 PlanName = "Free",
    //                 Price = 0,
    //                 DurationMonths = 0,
    //                 Description = "Basic access",
    //                 Features = new List<string>
    //                 {
    //                     "View basic train updates",
    //                     "Limited content access",
    //                     "Basic notifications"
    //                 }
    //             },
    //             new SubscriptionPlanViewModel
    //             {
    //                 PlanName = "Monthly",
    //                 Price = 10,
    //                 DurationMonths = 1,
    //                 Description = "Full access for 1 month",
    //                 Features = new List<string>
    //                 {
    //                     "Full access to all updates",
    //                     "Premium timetable images",
    //                     "Detailed route information",
    //                     "Priority notifications",
    //                     "Ad-free experience"
    //                 },
    //                 IsPopular = true
    //             },
    //             new SubscriptionPlanViewModel
    //             {
    //                 PlanName = "Premium",
    //                 Price = 999,
    //                 DurationMonths = 12,
    //                 Description = "Full access for 1 year",
    //                 Features = new List<string>
    //                 {
    //                     "Everything in Monthly plan",
    //                     "Save Rs.189 per year",
    //                     "Early access to new features",
    //                     "Priority customer support",
    //                     "Downloadable timetables"
    //                 }
    //             }
    //         };
    //     }
    // }

    // Placeholder controller to handle subscription routes during payment feature disabled period
    [Authorize]
    public class SubscriptionController : Controller
    {
        public IActionResult Index()
        {
            TempData["InfoMessage"] = "All features are currently free! Enjoy full access to all train updates.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Subscribe(string plan)
        {
            TempData["InfoMessage"] = "All features are currently free! No subscription needed.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult History()
        {
            TempData["InfoMessage"] = "Payment history is not available. All features are currently free.";
            return RedirectToAction("Index", "Home");
        }
    }
}
