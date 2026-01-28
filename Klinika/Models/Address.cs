namespace Klinika.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public int CityId { get; set; }
        public string? ApartmentNumber { get; set; }
        public string? AdditionalInfo { get; set; }
        
        // Navigation properties
        public City City { get; set; }
    }
}