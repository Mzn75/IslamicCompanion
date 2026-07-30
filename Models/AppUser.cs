namespace IslamicCompanion.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string PasswordHash { get; set; }
        public int CurrentStreak { get; set; } = 0;
        public int HighestStreak { get; set; } = 0;
        public int TotalAdhkarSessions { get; set; } = 0; 
        public int TotalDoneTasks { get; set; } = 0;
        public DateTime JoinedDate { get; set; } = DateTime.Now;

        // Tracking last login to help calculate streaks
        public DateTime? LastAdhkarDate { get; set; }
        public List<RoutineActivity> Activities { get; set; } = new List<RoutineActivity>();
    }
}
