using MyAPP.Models;
using MyAPP.ViewModels;

namespace MyAPP.Services
{
    public interface IAlertService
    {
        Task<List<Alert>> GetUserAlertsAsync(string userId);
        Task<Alert?> GetAlertByIdAsync(int id);
        Task<Alert> CreateAlertAsync(AlertViewModel model, string userId);
        Task<bool> DeleteAlertAsync(int id, string userId);
        Task<List<Alert>> GetPendingAlertsAsync();
        Task UpdateAlertStatusAsync(int alertId, bool isAvailable);
        
        // Admin functions
        Task<List<Alert>> GetAllAlertsAsync();
        Task<int> GetActiveAlertsCountAsync();
        Task<bool> MarkAlertAvailableAsync(int alertId);
        Task<bool> AdminDeleteAlertAsync(int alertId);
        Task<Alert> CreateBroadcastAlertAsync(AlertViewModel model, string adminUserId);
    }
}
