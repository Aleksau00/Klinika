using Klinika.DATA;
using Klinika.Models;
using Microsoft.EntityFrameworkCore;

namespace Klinika.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            return context;
        }

        public static ApplicationDbContext CreateContextWithData()
        {
            var context = CreateInMemoryContext();
            SeedTestData(context);
            return context;
        }

        private static void SeedTestData(ApplicationDbContext context)
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City",
                PostalCode = "11000",
                Country = "Serbia"
            };
            context.Cities.Add(city);
            context.SaveChanges();

            var address = new Address
            {
                Id = 1,
                StreetName = "Test Street",
                StreetNumber = "1",
                CityId = 1
            };
            context.Addresses.Add(address);
            context.SaveChanges();

            var clinic = new Clinic
            {
                Id = 1,
                Name = "Test Clinic",
                AddressId = 1,
                PhoneNumber = "011-123-4567",
                Email = "test@clinic.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Clinics.Add(clinic);
            context.SaveChanges();

            var doctor = new Doctor
            {
                Id = 1,
                Email = "doctor@test.com",
                FirstName = "Test",
                LastName = "Doctor",
                PhoneNumber = "060-111-1111",
                JMBG = "1234567890123",
                Gender = "M",
                DateOfBirth = new DateTime(1980, 1, 1),
                AddressId = 1,
                ClinicId = 1,
                PasswordHash = "hash",
                Specialty = "Cardiology",
                LicenseNumber = "LIC-001",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Doctors.Add(doctor);
            context.SaveChanges();

            var secretary = new Secretary
            {
                Id = 2,
                Email = "secretary@test.com",
                FirstName = "Test",
                LastName = "Secretary",
                PhoneNumber = "060-222-2222",
                JMBG = "9876543210987",
                Gender = "F",
                DateOfBirth = new DateTime(1990, 1, 1),
                AddressId = 1,
                ClinicId = 1,
                PasswordHash = "hash",
                Qualification = "Medical Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Secretaries.Add(secretary);
            context.SaveChanges();

            var patient = new Patient
            {
                Id = 3,
                Email = "patient@test.com",
                FirstName = "Test",
                LastName = "Patient",
                PhoneNumber = "060-333-3333",
                JMBG = "1111111111111",
                Gender = "M",
                DateOfBirth = new DateTime(1995, 1, 1),
                AddressId = 1,
                BloodType = "A+",
                NoShowCount = 0,
                CreatedAt = DateTime.UtcNow
            };
            context.Patients.Add(patient);
            context.SaveChanges();

            var slot = new AppointmentSlot
            {
                Id = 1,
                DoctorId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(9, 15),
                IsAvailable = true
            };
            context.AppointmentSlots.Add(slot);
            context.SaveChanges();
        }
    }
}