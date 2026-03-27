using MyAPP.Models;

namespace MyAPP.Services
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(string userId, string planName, decimal amount, int? subscriptionId = null);
        Task<Payment> CreateUpiPaymentAsync(string userId, string planName, decimal amount, string payerUpiId, string paymentMethod, int? subscriptionId = null);
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<List<Payment>> GetUserPaymentsAsync(string userId);
        Task<List<Payment>> GetAllPaymentsAsync();
        Task<bool> ProcessPaymentAsync(int paymentId);
        Task<decimal> GetTotalRevenueAsync();
        string GetMerchantUpiId();
        string GetMerchantName();
        string GenerateUpiPaymentLink(decimal amount, string transactionId);
    }
}
