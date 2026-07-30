namespace IslamicCompanion.Models
{
    public class AdhkarViewModel
    {
        public List<DhikrItem> MorningAdhkar {  get; set; } = new List<DhikrItem>();
        public List<DhikrItem> EveningAdhkar {  get; set; } = new List<DhikrItem>();
        public List<DhikrItem> SleepAdhkar {  get; set; } = new List<DhikrItem>();
        public List<DhikrItem> TravelAdhkar {  get; set; } = new List<DhikrItem>();

    }
}
