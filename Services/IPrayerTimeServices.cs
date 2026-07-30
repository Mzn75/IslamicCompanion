using System;
using System.Threading.Tasks;
using IslamicCompanion.Models;

namespace IslamicCompanion.Services
{
    public interface IPrayerTimeService
    {
        // Removed the hardcoded defaults
        Task<DailyPrayerTimes> GetPrayerTimesAsync(DateTime date, double lat, double lng);

        // Added userLocalTime to compare against the user's clock, not the server's clock
        TimeSpan GetNextPrayerTime(DailyPrayerTimes times, TimeSpan userLocalTime, out string prayerName);
    }
}