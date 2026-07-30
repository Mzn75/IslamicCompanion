using System;

namespace IslamicCompanion.Models
{
    public class DashboardViewModel
    {
        // 1. Flag to indicate if location permission is needed
        public bool NeedsLocation { get; set; }
        // 2. Hijri Date for the header
        public string HijriDate { get; set; }
        public DailyPrayerTimes TodayPrayers { get; set; }
        public string NextPrayerName { get; set; }
        public TimeSpan NextPrayerTime { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public QuranVerse DailyVerse { get; set; }
        public List<DailyTaskItem> DailyTasks { get; set; }
    }
}
