namespace Klinika.Models.DTOs
{
    public class AddPatientAllergenRequest
    {
        public int PatientId { get; set; }
        public int AllergenId { get; set; }
        public DateTime DiagnosedDate { get; set; }
        public string? Notes { get; set; }
    }
}