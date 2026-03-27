using Microsoft.EntityFrameworkCore;
using MyAPP.Data;
using MyAPP.Models;

namespace MyAPP.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Payment> CreatePaymentAsync(string userId, string planName, decimal amount, int? subscriptionId = null)
        {
            var payment = new Payment
            {
                UserId = userId,
                PlanName = planName,
                Amount = amount,
                SubscriptionId = subscriptionId,
                PaymentDate = DateTime.UtcNow,
                Status = "Pending",
                TransactionId = GenerateTransactionId(),
                MerchantUpiId = _configuration["UpiPayment:MerchantUpiId"] ?? "trainupdates@ybl"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return payment;
        }

        public async Task<Payment> CreateUpiPaymentAsync(string userId, string planName, decimal amount, string payerUpiId, string paymentMethod, int? subscriptionId = null)
        {
            var payment = new Payment
            {
                UserId = userId,
                PlanName = planName,
                Amount = amount,
                SubscriptionId = subscriptionId,
                PaymentDate = DateTime.UtcNow,
                Status = "Pending",
                TransactionId = GenerateTransactionId(),
                PaymentMethod = paymentMethod,
                PayerUpiId = payerUpiId,
                MerchantUpiId = _configuration["UpiPayment:MerchantUpiId"] ?? "trainupdates@ybl"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return payment;
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Subscription)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Payment>> GetUserPaymentsAsync(string userId)
        {
            return await _context.Payments
                .Include(p => p.Subscription)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Subscription)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<bool> ProcessPaymentAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
                return false;

            // Simulate UPI payment processing (always successful for demo)
            payment.Status = "Completed";
            await _context.SaveChangesAsync();

            // Create notification
            var notification = new Notification
            {
                UserId = payment.UserId,
                Title = "Payment Successful!",
                Message = $"Your UPI payment of ?{payment.Amount} for {payment.PlanName} plan has been processed successfully via {payment.PaymentMethod}.",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Payments
                .Where(p => p.Status == "Completed")
                .SumAsync(p => p.Amount);
        }

        public string GetMerchantUpiId()
        {
            return _configuration["UpiPayment:MerchantUpiId"] ?? "trainupdates@ybl";
        }

        public string GetMerchantName()
        {
            return _configuration["UpiPayment:MerchantName"] ?? "Train Updates";
        }

        public string GenerateUpiPaymentLink(decimal amount, string transactionId)
        {
            var merchantUpiId = GetMerchantUpiId();
            var merchantName = GetMerchantName();
            
            // UPI Deep Link format
            return $"upi://pay?pa={merchantUpiId}&pn={Uri.EscapeDataString(merchantName)}&am={amount}&cu=INR&tn={Uri.EscapeDataString($"Payment for {transactionId}")}";
        }

        private string GenerateTransactionId()
        {
            return $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }
    }
}
