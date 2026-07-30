using IslamicCompanion.Models;
using IslamicCompanion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IslamicCompanion.Services
{
    public class PrayerTimeService : IPrayerTimeService
    {
        private readonly HttpClient _httpClient;

        public PrayerTimeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DailyPrayerTimes> GetPrayerTimesAsync(DateTime date, double lat, double lng)
        {
            var url = $"https://api.aladhan.com/v1/timings/{date:dd-MM-yyyy}?latitude={lat}&longitude={lng}"; ;

            var response = await _httpClient.GetFromJsonAsync<AlAdhanResponse>(url);

            if (response?.Data?.Timings == null)
            {
                throw new Exception("Failed to retrieve prayer times from the API.");
            }

            var timings = response.Data.Timings;

            return new DailyPrayerTimes
            {
                Date = date,
                Fajr = ParseTime(timings.Fajr),
                Sunrise = ParseTime(timings.Sunrise),
                Dhuhr = ParseTime(timings.Dhuhr),
                Asr = ParseTime(timings.Asr),
                Maghrib = ParseTime(timings.Maghrib),
                Isha = ParseTime(timings.Isha)
            };
        }

        public TimeSpan GetNextPrayerTime(DailyPrayerTimes times, TimeSpan userLocalTime, out string prayerName)
        {
            var now = userLocalTime;

            var prayers = new Dictionary<string, TimeSpan>
            {
                { "Fajr", times.Fajr },
                { "Dhuhr", times.Dhuhr },
                { "Asr", times.Asr },
                { "Maghrib", times.Maghrib },
                { "Isha", times.Isha }
            };

            foreach (var prayer in prayers.OrderBy(p => p.Value))
            {
                if (now < prayer.Value)
                {
                    prayerName = prayer.Key;
                    return prayer.Value;
                }
            }

            // If all prayers today have passed, next is Fajr tomorrow
            prayerName = "Fajr";
            return times.Fajr.Add(TimeSpan.FromHours(24));
        }

        // Helper to strip the timezone (e.g. "05:15 (EEST)" -> "05:15")
        private TimeSpan ParseTime(string apiTime)
        {
            var cleanTime = apiTime.Split(' ')[0];
            return TimeSpan.Parse(cleanTime);
        }
    }
}