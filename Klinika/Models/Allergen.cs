namespace Klinika.Models
{
    public class Allergen
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        // Many-to-many relationship with Patients
        public ICollection<PatientAllergen> PatientAllergens { get; set; } = new List<PatientAllergen>();
    }
}