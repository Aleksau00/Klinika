namespace Klinika.Models
{
    public class PatientAllergen
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int AllergenId { get; set; }
        public Allergen Allergen { get; set; }

        public DateTime DiagnosedDate { get; set; }
        public string? Notes { get; set; }
    }
}