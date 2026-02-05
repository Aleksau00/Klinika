namespace Klinika.Models.DTOs
{
    public class PatientAllergenDto
    {
        public int AllergenId { get; set; }
        public string AllergenName { get; set; }
        public string? AllergenDescription { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime DiagnosedDate { get; set; }
        public string? Notes { get; set; }
    }
}