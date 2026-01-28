using Klinika.Models.DTOs;

namespace Klinika.Models.DTOs
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string? ApartmentNumber { get; set; }
        public string? AdditionalInfo { get; set; }
        public CityDto City { get; set; }
        
        public string FullAddress => $"{StreetName} {StreetNumber}" +
            (string.IsNullOrEmpty(ApartmentNumber) ? "" : $"/{ApartmentNumber}") +
            $", {City.Name} {City.PostalCode}, {City.Country}";
    }
}