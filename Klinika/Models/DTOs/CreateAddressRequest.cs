namespace Klinika.Models.DTOs
{
    public class CreateAddressRequest
    {
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public int CityId { get; set; }
        public string? ApartmentNumber { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}