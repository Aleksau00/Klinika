namespace Klinika.Models
{
    public class Vaccination
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        
        // Navigation
        public ICollection<VaccinationRecord> VaccinationRecords { get; set; } = new List<VaccinationRecord>();
    }
}