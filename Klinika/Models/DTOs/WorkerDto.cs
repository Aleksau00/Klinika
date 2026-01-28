namespace Klinika.Models.DTOs
{
    public class WorkerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public int? ClinicId { get; set; }
        public string? ClinicName { get; set; }
        public bool IsActive { get; set; }
        
        // Role-specific fields
        public string? Specialty { get; set; } // Doctor
        public string? LicenseNumber { get; set; } // Doctor
        public string? Qualification { get; set; } // Secretary
        public string? SeniorityLevel { get; set; } // Administrator
    }
}