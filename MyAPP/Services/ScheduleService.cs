using MyAPP.Models;

namespace MyAPP.Services
{
    public class ScheduleService : IScheduleService
    {
        private static readonly Dictionary<string, DayOfWeek> DayMap = new()
        {
            { "Sun", DayOfWeek.Sunday },
            { "Mon", DayOfWeek.Monday },
            { "Tue", DayOfWeek.Tuesday },
            { "Wed", DayOfWeek.Wednesday },
            { "Thu", DayOfWeek.Thursday },
            { "Fri", DayOfWeek.Friday },
            { "Sat", DayOfWeek.Saturday }
        };

        private static readonly Dictionary<DayOfWeek, string> ReverseDayMap = new()
        {
            { DayOfWeek.Sunday, "Sun" },
            { DayOfWeek.Monday, "Mon" },
            { DayOfWeek.Tuesday, "Tue" },
            { DayOfWeek.Wednesday, "Wed" },
            { DayOfWeek.Thursday, "Thu" },
            { DayOfWeek.Friday, "Fri" },
            { DayOfWeek.Saturday, "Sat" }
        };

        public List<string> GetDayNames()
        {
            return new List<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        }

        public string GetScheduleDisplayText(TrainUpdate train)
        {
            var days = train.ScheduleDays?.Select(d => d.DayOfWeek).ToList() ?? new List<string>();
            var daysText = GetDaysDisplayText(days);

            return train.ScheduleType switch
            {
                ScheduleType.Daily => GetDailyDisplayText(train, daysText),
                ScheduleType.Weekly => GetWeeklyDisplayText(train, days),
                ScheduleType.CustomDays => GetCustomDaysDisplayText(train, daysText),
                ScheduleType.OneTime => GetOneTimeDisplayText(train, days),
                ScheduleType.DateRange => GetDateRangeDisplayText(train, daysText),
                _ => "Schedule not set"
            };
        }

        private string GetDaysDisplayText(List<string> days)
        {
            if (days.Count == 0) return "No days";
            if (days.Count == 7) return "All Days";
            
            var orderedDays = OrderDays(days);
            return string.Join(", ", orderedDays);
        }

        private string GetDailyDisplayText(TrainUpdate train, string daysText)
        {
            var dateRange = "";
            if (train.StartDate.HasValue)
            {
                dateRange = $" (from {train.StartDate.Value:MMM dd}";
                if (train.EndDate.HasValue)
                {
                    dateRange += $" to {train.EndDate.Value:MMM dd}";
                }
                dateRange += ")";
            }

            if (daysText == "All Days")
            {
                return $"Runs Daily{dateRange}";
            }
            return $"Runs on {daysText}{dateRange}";
        }

        private string GetWeeklyDisplayText(TrainUpdate train, List<string> days)
        {
            if (days.Any())
            {
                var day = days.First();
                var fullDayName = GetFullDayName(day);
                var dateRange = "";
                if (train.StartDate.HasValue)
                {
                    dateRange = $" (from {train.StartDate.Value:MMM dd})";
                }
                return $"Runs every {fullDayName}{dateRange}";
            }
            return "Runs Weekly";
        }

        private string GetCustomDaysDisplayText(TrainUpdate train, string daysText)
        {
            var dateRange = "";
            if (train.StartDate.HasValue)
            {
                dateRange = $" (from {train.StartDate.Value:MMM dd})";
            }
            return $"Runs on {daysText}{dateRange}";
        }

        private string GetOneTimeDisplayText(TrainUpdate train, List<string> days)
        {
            var runDate = train.StartDate ?? train.TravelDate;
            var dayText = days.Any() ? $" ({days.First()})" : "";
            return $"Runs on {runDate:MMMM dd, yyyy}{dayText}";
        }

        private string GetDateRangeDisplayText(TrainUpdate train, string daysText)
        {
            if (train.StartDate.HasValue && train.EndDate.HasValue)
            {
                return $"Runs on {daysText} from {train.StartDate.Value:MMM dd} to {train.EndDate.Value:MMM dd}";
            }
            return "Date range not set";
        }

        private string GetFullDayName(string shortDay)
        {
            return shortDay switch
            {
                "Sun" => "Sunday",
                "Mon" => "Monday",
                "Tue" => "Tuesday",
                "Wed" => "Wednesday",
                "Thu" => "Thursday",
                "Fri" => "Friday",
                "Sat" => "Saturday",
                _ => shortDay
            };
        }

        private List<string> OrderDays(List<string> days)
        {
            var dayOrder = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            return days.OrderBy(d => Array.IndexOf(dayOrder, d)).ToList();
        }

        public DateTime? GetNextRunDate(TrainUpdate train)
        {
            var today = DateTime.Today;
            var scheduledDays = train.ScheduleDays?
                .Select(d => DayMap.GetValueOrDefault(d.DayOfWeek, DayOfWeek.Monday))
                .ToHashSet() ?? new HashSet<DayOfWeek>();

            if (!scheduledDays.Any())
                return null;

            return train.ScheduleType switch
            {
                ScheduleType.Daily => GetNextDailyRunDate(train, today, scheduledDays),
                ScheduleType.Weekly => GetNextWeeklyRunDate(train, today, scheduledDays),
                ScheduleType.CustomDays => GetNextCustomDaysRunDate(train, today, scheduledDays),
                ScheduleType.OneTime => GetNextOneTimeRunDate(train, today, scheduledDays),
                ScheduleType.DateRange => GetNextDateRangeRunDate(train, today, scheduledDays),
                _ => null
            };
        }

        private DateTime? GetNextDailyRunDate(TrainUpdate train, DateTime today, HashSet<DayOfWeek> scheduledDays)
        {
            var startDate = train.StartDate ?? today;
            var endDate = train.EndDate;

            // Start checking from today or start date, whichever is later
            var checkDate = today >= startDate ? today : startDate;

            // Check up to 7 days ahead
            for (int i = 0; i < 7; i++)
            {
                var date = checkDate.AddDays(i);
                
                // Check if within end date
                if (endDate.HasValue && date > endDate.Value)
                    return null;

                if (scheduledDays.Contains(date.DayOfWeek))
                    return date;
            }

            return null;
        }

        private DateTime? GetNextWeeklyRunDate(TrainUpdate train, DateTime today, HashSet<DayOfWeek> scheduledDays)
        {
            if (!scheduledDays.Any())
                return null;

            var targetDay = scheduledDays.First();
            var startDate = train.StartDate ?? today;
            var checkDate = today >= startDate ? today : startDate;

            // Check up to 7 days ahead
            for (int i = 0; i < 7; i++)
            {
                var date = checkDate.AddDays(i);
                if (date.DayOfWeek == targetDay)
                    return date;
            }

            return null;
        }

        private DateTime? GetNextCustomDaysRunDate(TrainUpdate train, DateTime today, HashSet<DayOfWeek> scheduledDays)
        {
            var startDate = train.StartDate ?? today;
            var checkDate = today >= startDate ? today : startDate;

            // Check up to 7 days ahead
            for (int i = 0; i < 7; i++)
            {
                var date = checkDate.AddDays(i);
                if (scheduledDays.Contains(date.DayOfWeek))
                    return date;
            }

            return null;
        }

        private DateTime? GetNextOneTimeRunDate(TrainUpdate train, DateTime today, HashSet<DayOfWeek> scheduledDays)
        {
            var runDate = train.StartDate ?? train.TravelDate;
            
            // Check if the date is today or in future and matches the selected day
            if (runDate.Date >= today && scheduledDays.Contains(runDate.DayOfWeek))
                return runDate.Date;

            return null;
        }

        private DateTime? GetNextDateRangeRunDate(TrainUpdate train, DateTime today, HashSet<DayOfWeek> scheduledDays)
        {
            if (!train.StartDate.HasValue || !train.EndDate.HasValue)
                return null;

            // Start checking from today or start date, whichever is later
            var checkDate = today >= train.StartDate.Value ? today : train.StartDate.Value;

            // Check each day within range
            while (checkDate <= train.EndDate.Value)
            {
                if (scheduledDays.Contains(checkDate.DayOfWeek))
                    return checkDate;
                checkDate = checkDate.AddDays(1);
            }

            return null;
        }

        public string GetScheduleBadgeClass(ScheduleType scheduleType)
        {
            return scheduleType switch
            {
                ScheduleType.Daily => "bg-success",
                ScheduleType.Weekly => "bg-primary",
                ScheduleType.CustomDays => "bg-warning text-dark",
                ScheduleType.OneTime => "bg-danger",
                ScheduleType.DateRange => "bg-info",
                _ => "bg-secondary"
            };
        }

        public string GetScheduleBadgeText(ScheduleType scheduleType)
        {
            return scheduleType switch
            {
                ScheduleType.Daily => "Daily",
                ScheduleType.Weekly => "Weekly",
                ScheduleType.CustomDays => "Custom",
                ScheduleType.OneTime => "One-Time",
                ScheduleType.DateRange => "Date Range",
                _ => "Unknown"
            };
        }

        public bool IsRunningToday(TrainUpdate train)
        {
            var nextRun = GetNextRunDate(train);
            return nextRun.HasValue && nextRun.Value.Date == DateTime.Today;
        }
    }
}
