using IslamicCompanion.Models;
using IslamicCompanion.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IslamicCompanion.Services
{
    public class QuranApiService : IQuranApiService
    {
        private readonly HttpClient _httpClient;

        public QuranApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private readonly string[] _bestVerses = new[]
        {
            "2:152",
            "2:255",
            "2:286",
            "3:190",
            "13:28",
            "14:7",
            "24:35",
            "39:53",
            "40:60",
            "50:16",
            "55:60",
            "94:6",
            "93:5",
            "8:33",
            "2:153",
            "11:115",
            "65:3"
        };

        // A dictionary mapping the Surah number to both its English and Arabic names
        private readonly Dictionary<int, (string , string)> _surahNames = new()
        {
            { 2, ("Al-Baqarah", "البقرة") },
            { 3, ("Ali 'Imran", "آل عمران") },
            { 13, ("Ar-Ra'd", "الرعد") },
            { 14, ("Ibrahim", "إبراهيم") },
            { 24, ("An-Nur", "النور") },
            { 39, ("Az-Zumar", "الزمر") },
            { 40, ("Ghafir", "غافر") },
            { 50, ("Qaf", "ق") },
            { 55, ("Ar-Rahman", "الرحمن") },
            { 94, ("Ash-Sharh", "الشرح") },
            { 93, ("Ad-Duha", "الضحى") },
            { 8, ("Al-Anfal", "الأنفال") },
            { 11, ("Hud", "هود") },
            { 65, ("At-Talaq", "الطلاق") }
        };

        public async Task<QuranVerse> GetDailyVerseAsync()
        {
            // Seeded randomizer: Picks the exact same verse all day for everyone
            var today = DateTime.UtcNow.Date;
            int seed = today.Year * 10000 + today.Month * 100 + today.Day;
            var random = new Random(seed);

            string targetKey = _bestVerses[random.Next(_bestVerses.Length)];
            return await FetchVerseByKeyAsync(targetKey);
        }

        public async Task<QuranVerse> GetRandomVerseAsync()
        {
            // Unseeded randomizer: Picks a new verse every time the button is clicked
            var random = new Random();

            string targetKey = _bestVerses[random.Next(_bestVerses.Length)];
            return await FetchVerseByKeyAsync(targetKey);
        }

        // Helper method to actually call the API
        private async Task<QuranVerse> FetchVerseByKeyAsync(string verseKey)
        {
            // 131 is Dr. Mustafa Khattab (The Clear Quran) translation
            var verseUrl = $"https://api.quran.com/api/v4/verses/by_key/{verseKey}?translations=131&fields=text_uthmani";
            var response = await _httpClient.GetFromJsonAsync<QuranComResponse>(verseUrl);

            if (response?.Verse == null)
            {
                throw new Exception("Failed to fetch verse from Quran.com");
            }

            var parts = verseKey.Split(':');
            int surahNum = int.Parse(parts[0]);
            int ayahNum = int.Parse(parts[1]);

            var surahInfo = _surahNames.TryGetValue(surahNum, out var names) ? names : ($"Surah {surahNum}", $"سورة {surahNum}");

            return new QuranVerse
            {
                SurahNumber = surahNum,
                AyahNumber = ayahNum,
                SurahName = surahInfo.Item1,
                SurahNameArabic = surahInfo.Item2,
                ArabicText = response.Verse.TextUthmani,
                Translation = response.Verse.Translations?.FirstOrDefault()?.Text ?? "Translation missing"
            };
        }

    }
}