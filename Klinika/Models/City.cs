namespace Klinika.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; } = "Serbia"; // Default country
        
        // Navigation property
        public ICollection<Address> Addresses { get; set; }
    }
}