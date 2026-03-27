using Microsoft.EntityFrameworkCore;
using MyAPP.Data;
using MyAPP.Models;
using MyAPP.ViewModels;

namespace MyAPP.Services
{
    public class TrainUpdateService : ITrainUpdateService
    {
        private readonly ApplicationDbContext _context;

        public TrainUpdateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TrainUpdate>> GetAllUpdatesAsync(string? searchTerm = null, DateTime? filterDate = null,
            string? fromStation = null, string? toStation = null, int page = 1, int pageSize = 10)
        {
            var query = _context.TrainUpdates
                .Include(t => t.CreatedBy)
                .Include(t => t.ScheduleDays)
                .Where(t => t.IsActive)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(searchTerm) ||
                    t.Description.ToLower().Contains(searchTerm) ||
                    t.TrainNumber.ToLower().Contains(searchTerm) ||
                    t.FromStation.ToLower().Contains(searchTerm) ||
                    t.ToStation.ToLower().Contains(searchTerm));
            }

            // Apply date filter - check against StartDate, EndDate, or TravelDate
            if (filterDate.HasValue)
            {
                query = query.Where(t => 
                    t.TravelDate.Date == filterDate.Value.Date ||
                    (t.StartDate.HasValue && t.StartDate.Value.Date == filterDate.Value.Date) ||
                    (t.StartDate.HasValue && t.EndDate.HasValue && 
                     filterDate.Value.Date >= t.StartDate.Value.Date && 
                     filterDate.Value.Date <= t.EndDate.Value.Date));
            }

            // Apply from station filter
            if (!string.IsNullOrWhiteSpace(fromStation))
            {
                query = query.Where(t => t.FromStation.ToLower().Contains(fromStation.ToLower()));
            }

            // Apply to station filter
            if (!string.IsNullOrWhiteSpace(toStation))
            {
                query = query.Where(t => t.ToStation.ToLower().Contains(toStation.ToLower()));
            }

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? searchTerm = null, DateTime? filterDate = null,
            string? fromStation = null, string? toStation = null)
        {
            var query = _context.TrainUpdates
                .Where(t => t.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(searchTerm) ||
                    t.Description.ToLower().Contains(searchTerm) ||
                    t.TrainNumber.ToLower().Contains(searchTerm) ||
                    t.FromStation.ToLower().Contains(searchTerm) ||
                    t.ToStation.ToLower().Contains(searchTerm));
            }

            if (filterDate.HasValue)
            {
                query = query.Where(t => 
                    t.TravelDate.Date == filterDate.Value.Date ||
                    (t.StartDate.HasValue && t.StartDate.Value.Date == filterDate.Value.Date) ||
                    (t.StartDate.HasValue && t.EndDate.HasValue && 
                     filterDate.Value.Date >= t.StartDate.Value.Date && 
                     filterDate.Value.Date <= t.EndDate.Value.Date));
            }

            if (!string.IsNullOrWhiteSpace(fromStation))
            {
                query = query.Where(t => t.FromStation.ToLower().Contains(fromStation.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(toStation))
            {
                query = query.Where(t => t.ToStation.ToLower().Contains(toStation.ToLower()));
            }

            return await query.CountAsync();
        }

        public async Task<TrainUpdate?> GetUpdateByIdAsync(int id)
        {
            return await _context.TrainUpdates
                .Include(t => t.CreatedBy)
                .Include(t => t.ScheduleDays)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
        }

        public async Task<TrainUpdate> CreateUpdateAsync(TrainUpdateViewModel model, string userId, string? imagePath = null)
        {
            var trainUpdate = new TrainUpdate
            {
                Title = model.Title,
                Description = model.Description,
                TrainNumber = model.TrainNumber,
                FromStation = model.FromStation,
                ToStation = model.ToStation,
                TravelDate = model.StartDate ?? model.TravelDate,
                ScheduleType = model.ScheduleType,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsPremium = model.IsPremium,
                ImagePath = imagePath,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.TrainUpdates.Add(trainUpdate);
            await _context.SaveChangesAsync();

            // Add schedule days for Weekly or CustomDays
            await SaveScheduleDaysAsync(trainUpdate.Id, model);

            return trainUpdate;
        }

        public async Task<bool> UpdateAsync(TrainUpdateViewModel model, string? imagePath = null)
        {
            var trainUpdate = await _context.TrainUpdates
                .Include(t => t.ScheduleDays)
                .FirstOrDefaultAsync(t => t.Id == model.Id);
                
            if (trainUpdate == null)
                return false;

            trainUpdate.Title = model.Title;
            trainUpdate.Description = model.Description;
            trainUpdate.TrainNumber = model.TrainNumber;
            trainUpdate.FromStation = model.FromStation;
            trainUpdate.ToStation = model.ToStation;
            trainUpdate.TravelDate = model.StartDate ?? model.TravelDate;
            trainUpdate.ScheduleType = model.ScheduleType;
            trainUpdate.StartDate = model.StartDate;
            trainUpdate.EndDate = model.EndDate;
            trainUpdate.IsPremium = model.IsPremium;
            trainUpdate.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(imagePath))
            {
                trainUpdate.ImagePath = imagePath;
            }

            // Remove existing schedule days and add new ones
            _context.TrainScheduleDays.RemoveRange(trainUpdate.ScheduleDays);
            await _context.SaveChangesAsync();

            await SaveScheduleDaysAsync(trainUpdate.Id, model);

            return true;
        }

        private async Task SaveScheduleDaysAsync(int trainUpdateId, TrainUpdateViewModel model)
        {
            var daysToAdd = new List<TrainScheduleDay>();

            // Save days for ALL schedule types
            if (model.SelectedDays?.Any() == true)
            {
                foreach (var day in model.SelectedDays)
                {
                    daysToAdd.Add(new TrainScheduleDay
                    {
                        TrainUpdateId = trainUpdateId,
                        DayOfWeek = day
                    });
                }
            }

            if (daysToAdd.Any())
            {
                _context.TrainScheduleDays.AddRange(daysToAdd);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var trainUpdate = await _context.TrainUpdates.FindAsync(id);
            if (trainUpdate == null)
                return false;

            trainUpdate.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
