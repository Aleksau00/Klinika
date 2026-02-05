namespace Klinika.Models.DTOs
{
    public class AdministerVaccinationRequest
    {
        public int PreventiveAppointmentId { get; set; }
        public int VaccinationId { get; set; }
        public string? Notes { get; set; }
    }
}