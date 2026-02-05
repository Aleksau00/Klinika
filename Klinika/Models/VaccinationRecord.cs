namespace Klinika.Models
{
    public class VaccinationRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        
        public int VaccinationId { get; set; }
        public Vaccination Vaccination { get; set; }
        
        public DateTime AdministeredDate { get; set; }
        public string? Notes { get; set; }
        
        public int AdministeredByDoctorId { get; set; }
        public Doctor AdministeredByDoctor { get; set; }
        
        public int? PreventiveAppointmentId { get; set; }
        public PreventiveAppointment? PreventiveAppointment { get; set; }
    }
}