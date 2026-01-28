namespace Klinika.Models.DTOs
{
    public class CreateWeeklySlotsRequest
    {
        public int DoctorId { get; set; }
        public List<TimeOnly> DailyStartTimes { get; set; } = new List<TimeOnly>
        {
            new TimeOnly(8, 0),   // 8:00 AM
            new TimeOnly(9, 0),   // 9:00 AM
            new TimeOnly(10, 0),  // etc...
            new TimeOnly(11, 0),
            new TimeOnly(14, 0),  // After lunch
            new TimeOnly(15, 0),
            new TimeOnly(16, 0)
        };
    }
}