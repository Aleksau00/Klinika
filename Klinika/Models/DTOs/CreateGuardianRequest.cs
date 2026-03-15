namespace Klinika.Models.DTOs
{
    public class CreateGuardianRequest
    {
        public string? Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string JMBG { get; set; }
        public required string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? AddressId { get; set; }
    }

    public class UpdateGuardianContactRequest
    {
        public string? Email { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
