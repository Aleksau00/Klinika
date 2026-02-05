namespace Klinika.Models
{
    public class Patient : Person
    {
        public string BloodType { get; set; }
        public int? GuardianId { get; set; }
        public Guardian? Guardian { get; set; }
        public int NoShowCount { get; set; } = 0;

        // Navigation to appointments (base type)
        public ICollection<Appointment> Appointments { get; set; }

        // Many-to-many relationship with Allergens
        public ICollection<PatientAllergen> PatientAllergens { get; set; } = new List<PatientAllergen>();

        public ICollection<VaccinationRecord> VaccinationRecords { get; set; } = new List<VaccinationRecord>();
    }
}