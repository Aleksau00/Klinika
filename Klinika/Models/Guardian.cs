namespace Klinika.Models
{
    public class Guardian : Person
    {
        public ICollection<Patient> Children { get; set; } = new List<Patient>();
    }
}
