using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using IslamicCompanion.Models;
using IslamicCompanion.Services;
using IslamicCompanion.Data; // Required for the database context
using Microsoft.AspNetCore.Authorization;
using TimeZoneConverter;

namespace IslamicCompanion.Controllers
{
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class HomeController : Controller
    {
        private readonly IPrayerTimeService _prayerService;
        private readonly IQuranApiService _quranService;

        // 1. Declare the SQL Database context
        private readonly ApplicationDbContext _context;

        // 2. Dependency Injection now includes ApplicationDbContext
        public HomeController(IPrayerTimeService prayerService, IQuranApiService quranService, ApplicationDbContext context)
        {
            _prayerService = prayerService;
            _quranService = quranService;
            _context = context; // Assign the database engine
        }

        [HttpGet]
        public async Task<IActionResult> GetRandomVerse()
        {
            var verse = await _quranService.GetRandomVerseAsync();
            return Json(verse);
        }

        public async Task<IActionResult> Index(double? lat, double? lng, string timeZone)
        {
            if (!lat.HasValue || !lng.HasValue || string.IsNullOrEmpty(timeZone))
            {
                return View(new DashboardViewModel { NeedsLocation = true });
            }

            TimeZoneInfo userZone = TimeZoneConverter.TZConvert.GetTimeZoneInfo(timeZone);
            var userLocalTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userZone);

            var prayers = await _prayerService.GetPrayerTimesAsync(userLocalTime.Date, lat.Value, lng.Value);
            var nextTime = _prayerService.GetNextPrayerTime(prayers, userLocalTime.TimeOfDay, out string nextName);

            TimeSpan remainingTime = nextTime - userLocalTime.TimeOfDay;
            if (remainingTime < TimeSpan.Zero)
            {
                remainingTime = remainingTime.Add(TimeSpan.FromHours(24));
            }

            var dailyVerse = await _quranService.GetDailyVerseAsync();

            var hijriCalendar = new System.Globalization.UmAlQuraCalendar();
            int hYear = hijriCalendar.GetYear(userLocalTime.Date);
            int hMonth = hijriCalendar.GetMonth(userLocalTime.Date);
            int hDay = hijriCalendar.GetDayOfMonth(userLocalTime.Date);
            string hijriDateFormatted = $"{hDay} {GetHijriMonthName(hMonth)} {hYear} AH";

            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var todayString = userLocalTime.ToString("yyyy-MM-dd");

            // 3. SQL REPLACEMENT: Get completed task IDs instantly from the database
            var completedTaskIds = _context.Activities
                .Where(t => t.User.Username == username && t.TaskDate == todayString && t.IsCompleted)
                .Select(t => t.TaskId)
                .ToList();

            var myDailyTasks = new List<DailyTaskItem>
            {
                new DailyTaskItem { TaskId = 1, TaskName = "أذكار الصباح" },
                new DailyTaskItem { TaskId = 2, TaskName = "القران الكريم" },
                new DailyTaskItem { TaskId = 3, TaskName = "أذكار بعد الصلاة" },
                new DailyTaskItem { TaskId = 4, TaskName = "صلاة السنة" },
                new DailyTaskItem { TaskId = 5, TaskName = "أذكار المساء" },
                new DailyTaskItem { TaskId = 6, TaskName = "قيام الليل" },
                new DailyTaskItem { TaskId = 7, TaskName = "أذكار النوم" }
            };

            foreach (var task in myDailyTasks)
            {
                task.IsCompleted = completedTaskIds.Contains(task.TaskId);
            }

            var model = new DashboardViewModel
            {
                NeedsLocation = false,
                TodayPrayers = prayers,
                NextPrayerName = nextName,
                NextPrayerTime = nextTime,
                RemainingTime = remainingTime,
                DailyVerse = dailyVerse,
                HijriDate = hijriDateFormatted,
                DailyTasks = myDailyTasks,
            };

            return View(model);
        }

        private string GetHijriMonthName(int month)
        {
            string[] months = { "", "Muharram", "Safar", "Rabi' al-Awwal", "Rabi' al-Thani",
                                "Jumada al-Awwal", "Jumada al-Thani", "Rajab", "Sha'ban",
                                "Ramadan", "Shawwal", "Dhu al-Qi'dah", "Dhu al-Hijjah" };
            return months[month];
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult ToggleDailyTask([FromBody] ToggleTaskRequest request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(User.Identity.Name))
                {
                    return Unauthorized();
                }

                var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var todayString = DateTime.Now.ToString("yyyy-MM-dd");

                // 4. SQL REPLACEMENT: Ask database if this specific task exists today
                var existingTask = _context.Activities.FirstOrDefault(t =>
                    t.User.Username == username &&
                    t.TaskId == request.TaskId &&
                    t.TaskName == request.TaskName &&
                    t.TaskDate == todayString);

                var currentUser = _context.Users.FirstOrDefault(u => u.Username == username);

                if (existingTask != null)
                {
                    // Update existing SQL row
                    existingTask.IsCompleted = request.IsCompleted;
                    existingTask.TaskName = request.TaskName;
                }
                else
                {
                    // Insert new SQL row
                    _context.Activities.Add(new RoutineActivity
                    {
                        AppUserId = currentUser.Id,
                        TaskId = request.TaskId,
                        TaskName = request.TaskName,
                        TaskDate = todayString,
                        IsCompleted = request.IsCompleted
                    });
                }

                // 5. Save all changes to the database at once
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("CRASH IN TOGGLE: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        public class ToggleTaskRequest
        {
            public int TaskId { get; set; }
            public string TaskName { get; set; }
            public string TaskDate { get; set; }
            public bool IsCompleted { get; set; }
        }

        public IActionResult UserHistory()
        {
            if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }

            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // 6. SQL REPLACEMENT: Filter, group, and sort directly on the database engine
            var historyData = _context.Activities
                .Where(t => t.User.Username == username && t.IsCompleted)
                .ToList()

                .GroupBy(t => t.TaskDate)
                .OrderByDescending(g => g.Key)
                .ToList();

            return View(historyData);
        }

        [Route("Home/Error404")]
        public IActionResult Error404()
        {
            return View("Error404");
        }
    }
}