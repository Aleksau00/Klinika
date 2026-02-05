namespace Klinika.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string JMBG { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }

  
}
