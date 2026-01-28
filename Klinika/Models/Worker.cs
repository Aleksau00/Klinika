using System.Text.Json.Serialization;

namespace Klinika.Models
{
    public class Worker : Person
    {
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        
        // Direct relationship - one worker belongs to one clinic
        public int? ClinicId { get; set; }
        public Clinic? Clinic { get; set; }
    }
}
