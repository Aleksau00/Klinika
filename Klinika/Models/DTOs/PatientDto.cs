namespace Klinika.Models.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string JMBG { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string BloodType { get; set; }
        public int NoShowCount { get; set; }
        public int? AddressId { get; set; }
        public int? GuardianId { get; set; }
        public string? GuardianName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePatientRequest
    {
        public string? Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string JMBG { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string BloodType { get; set; }
        public int? AddressId { get; set; }
        public int? GuardianId { get; set; }
    }

    public class UpdatePatientRequest
    {
        public string? Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string JMBG { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string BloodType { get; set; }
        public int? AddressId { get; set; }
        public int? GuardianId { get; set; }
    }
}