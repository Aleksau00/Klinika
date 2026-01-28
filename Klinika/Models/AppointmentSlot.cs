namespace Klinika.Models
{
    public class AppointmentSlot
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
        
        // Navigation property
        public Doctor Doctor { get; set; }
        
        // Constants for validation
        public static readonly TimeOnly WorkDayStart = new TimeOnly(8, 0);  // 8:00 AM
        public static readonly TimeOnly WorkDayEnd = new TimeOnly(20, 0);   // 8:00 PM
        public static readonly int SlotDurationMinutes = 15;
        
        // Validation method
        public bool IsValidSlot()
        {
            // Check if start time is in 15-minute increments
            if (StartTime.Minute % 15 != 0 || StartTime.Second != 0)
                return false;
            
            // Check if within working hours
            if (StartTime < WorkDayStart || EndTime > WorkDayEnd)
                return false;
            
            // Check if slot duration is exactly 15 minutes
            var duration = EndTime.ToTimeSpan() - StartTime.ToTimeSpan();
            if (duration.TotalMinutes != SlotDurationMinutes)
                return false;
            
            return true;
        }
    }
}