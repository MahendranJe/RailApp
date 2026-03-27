using MyAPP.Models;

namespace MyAPP.Services
{
    public interface IScheduleService
    {
        string GetScheduleDisplayText(TrainUpdate train);
        DateTime? GetNextRunDate(TrainUpdate train);
        string GetScheduleBadgeClass(ScheduleType scheduleType);
        string GetScheduleBadgeText(ScheduleType scheduleType);
        bool IsRunningToday(TrainUpdate train);
        List<string> GetDayNames();
    }
}
