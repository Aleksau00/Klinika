namespace Klinika.Models
{
    public class Doctor : Worker
    {
        public string Specialty { get; set; }
        public string LicenseNumber { get; set; }
    }
}
