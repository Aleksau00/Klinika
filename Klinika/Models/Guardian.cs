namespace Klinika.Models
{
    public class Guardian : Person
    {
        public List<Patient>? Children { get; set; }
    }
}
