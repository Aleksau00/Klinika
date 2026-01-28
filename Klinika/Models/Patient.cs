namespace Klinika.Models
{
    public class Patient : Person
    {
        public string BloodType { get; set; }
        public int? GuardianId { get; set; }
        public Guardian? Guardian { get; set; }
    }
}
