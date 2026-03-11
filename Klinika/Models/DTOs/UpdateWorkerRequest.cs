namespace Klinika.Models.DTOs
{
    public class UpdateWorkerRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string JMBG { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? ClinicId { get; set; }
        public string Role { get; set; }
        public string? NewPassword { get; set; }
        public string? SeniorityLevel { get; set; }
        public string? Specialty { get; set; }
        public string? LicenseNumber { get; set; }
        public string? Qualification { get; set; }
    }
}