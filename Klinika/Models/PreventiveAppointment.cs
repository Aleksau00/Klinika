namespace Klinika.Models
{
    public class PreventiveAppointment : Appointment
    {
        public string? PreventiveNotes { get; set; }
        public string? ChildDevelopmentNotes { get; set; } // For pediatric preventive care
        
        // Vaccination-specific (optional)
        public bool IsVaccination { get; set; } = false;
        public int? VaccinationId { get; set; }
        public Vaccination? Vaccination { get; set; }
        
        // Navigation to vaccination record
        public VaccinationRecord? VaccinationRecord { get; set; }
    }
}   