namespace Klinika.Models.DTOs
{
    public class CreateCustomSlotsRequest
    {
        public List<DateOnly> Dates { get; set; } = new List<DateOnly>(); // Specific dates
        public TimeOnly StartTime { get; set; } = new TimeOnly(9, 0);     // Work start (default 9 AM)
        public TimeOnly EndTime { get; set; } = new TimeOnly(17, 0);      // Work end (default 5 PM)
        public int DaysAhead { get; set; } = 7;                           // How many days to generate
        public bool SkipWeekends { get; set; } = false;                   // Skip Sat/Sun
    }
}