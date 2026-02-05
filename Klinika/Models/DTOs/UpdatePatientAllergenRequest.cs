namespace Klinika.Models.DTOs
{
    public class UpdatePatientAllergenRequest
    {
        public DateTime DiagnosedDate { get; set; }
        public string? Notes { get; set; }
    }
}