using MyAPP.Models;
using MyAPP.ViewModels;

namespace MyAPP.Services
{
    public interface ITrainUpdateService
    {
        Task<List<TrainUpdate>> GetAllUpdatesAsync(string? searchTerm = null, DateTime? filterDate = null, 
            string? fromStation = null, string? toStation = null, int page = 1, int pageSize = 10);
        Task<int> GetTotalCountAsync(string? searchTerm = null, DateTime? filterDate = null, 
            string? fromStation = null, string? toStation = null);
        Task<TrainUpdate?> GetUpdateByIdAsync(int id);
        Task<TrainUpdate> CreateUpdateAsync(TrainUpdateViewModel model, string userId, string? imagePath = null);
        Task<bool> UpdateAsync(TrainUpdateViewModel model, string? imagePath = null);
        Task<bool> DeleteAsync(int id);
    }
}
