using System;
using System.Text.Json.Serialization;

namespace IslamicCompanion.Models
{
    public class DailyPrayerTimes
    {
        public DateTime Date { get; set; }
        public TimeSpan Fajr { get; set; }
        public TimeSpan Sunrise { get; set; }
        public TimeSpan Dhuhr { get; set; }
        public TimeSpan Asr { get; set; }
        public TimeSpan Maghrib { get; set; }
        public TimeSpan Isha { get; set; }
    }

    // The JSON mapping classes for the API response
    public class AlAdhanResponse
    {
        [JsonPropertyName("data")]
        public AlAdhanData Data { get; set; }
    }

    public class AlAdhanData
    {
        [JsonPropertyName("timings")]
        public AlAdhanTimings Timings { get; set; }
    }

    public class AlAdhanTimings
    {
        [JsonPropertyName("Fajr")] public string Fajr { get; set; }
        [JsonPropertyName("Sunrise")] public string Sunrise { get; set; }
        [JsonPropertyName("Dhuhr")] public string Dhuhr { get; set; }
        [JsonPropertyName("Asr")] public string Asr { get; set; }
        [JsonPropertyName("Maghrib")] public string Maghrib { get; set; }
        [JsonPropertyName("Isha")] public string Isha { get; set; }
    }
}