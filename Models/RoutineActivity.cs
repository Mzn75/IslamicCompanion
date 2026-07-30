using System;

namespace IslamicCompanion.Models
{
    public class RoutineActivity
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDate { get; set; }
        public bool IsCompleted { get; set; }

        // The relational link back to the user
        public int AppUserId { get; set; }
        public AppUser User { get; set; }
    }
}
