namespace Klinika.Models.DTOs
{
    public class CreateCityRequest
    {
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; } = "Serbia";
    }
}
