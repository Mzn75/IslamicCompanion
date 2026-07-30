namespace IslamicCompanion.Models
{
    public class UserHistory
    {
        public string Username { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
