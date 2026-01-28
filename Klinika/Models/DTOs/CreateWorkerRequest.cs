namespace Klinika.Models.DTOs
{
    public class CreateWorkerRequest
    {
        // Person fields
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string JMBG { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        
        // Address information
        public int? AddressId { get; set; }  // Use existing address
        public CreateAddressRequest? NewAddress { get; set; }  // Or create new address
        
        // Clinic assignment
        public int? ClinicId { get; set; }
        
        // Worker creation
        public string TemporaryPassword { get; set; }
        public string Role { get; set; } // "Administrator", "Doctor", "Secretary"

        // Role-specific optional fields
        public string? SeniorityLevel { get; set; } // Administrator
        public string? Specialty { get; set; } // Doctor
        public string? LicenseNumber { get; set; } // Doctor
        public string? Qualification { get; set; } // Secretary
    }
}