namespace Klinika.Models
{
    public class PreventiveAppointment : Appointment
    {
        public string? PreventiveNotes { get; set; }  // General notes/advice
        // Future: Add vaccination tracking in Phase 4
        
        public PreventiveAppointment()
        {
            AppointmentType = AppointmentType.Preventive;
        }
    }
}