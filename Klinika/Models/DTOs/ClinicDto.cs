namespace Klinika.Models.DTOs
{
    public class ClinicDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AddressDto Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int WorkerCount { get; set; }
    }
    
    public class CreateClinicRequest
    {
        public string Name { get; set; }
        public int? AddressId { get; set; }
        public CreateAddressRequest? NewAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
    
    public class UpdateClinicRequest
    {
        public string Name { get; set; }
        public int AddressId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}