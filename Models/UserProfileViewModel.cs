using System;

namespace IslamicCompanion.Models
{
    public class UserProfileViewModel
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public int CurrentStreak { get; set; }
        public int HighestStreak { get; set; }
        public int TotalAdhkarSessions { get; set; }
        public int MorningCompletionRate { get; set; } // Percentage 0-100
        public int EveningCompletionRate { get; set; } // Percentage 0-100
        public DateTime JoinedDate { get; set; }
        public int DailyTaskScore { get; set; }
        public int TotalDoneTasks { get; set; }
    }
}