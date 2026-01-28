using Klinika.Models;
using Klinika.Models.DTOs;
using Klinika.Repositories;

namespace Klinika.Services
{
    internal class WorkerService : IWorkerService
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICityRepository _cityRepository;

        public WorkerService(
            IWorkerRepository workerRepository,
            IAddressRepository addressRepository,
            ICityRepository cityRepository)
        {
            _workerRepository = workerRepository;
            _addressRepository = addressRepository;
            _cityRepository = cityRepository;
        }

        public async Task<Worker?> GetByIdAsync(int id)
        {
            var worker = await _workerRepository.GetByIdAsync(id);
            if (worker == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }
            return worker;
        }

        public async Task<Worker> CreateAsync(CreateWorkerRequest request)
        {
            // Validate role
            if (request.Role != "Administrator" && request.Role != "Doctor" && request.Role != "Secretary")
            {
                throw new ArgumentException("Role must be Administrator, Doctor, or Secretary");
            }

            // Check if email already exists
            var existingWorker = await _workerRepository.GetByEmailAsync(request.Email);
            if (existingWorker != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Handle address
            int? addressId = await ResolveAddressAsync(request);

            // Create based on role
            Worker worker = request.Role switch
            {
                "Administrator" => new Administrator
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    JMBG = request.JMBG,
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth,
                    AddressId = addressId,
                    ClinicId = request.ClinicId, // Assign to clinic
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.TemporaryPassword),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    SeniorityLevel = request.SeniorityLevel
                },

                "Doctor" => new Doctor
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    JMBG = request.JMBG,
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth,
                    AddressId = addressId,
                    ClinicId = request.ClinicId, // Assign to clinic
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.TemporaryPassword),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Specialty = request.Specialty,
                    LicenseNumber = request.LicenseNumber
                },

                "Secretary" => new Secretary
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    JMBG = request.JMBG,
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth,
                    AddressId = addressId,
                    ClinicId = request.ClinicId, // Assign to clinic
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.TemporaryPassword),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Qualification = request.Qualification
                },

                _ => throw new ArgumentException("Invalid role")
            };

            // Call appropriate repository method
            return request.Role switch
            {
                "Administrator" => await _workerRepository.CreateAdministratorAsync((Administrator)worker),
                "Doctor" => await _workerRepository.CreateDoctorAsync((Doctor)worker),
                "Secretary" => await _workerRepository.CreateSecretaryAsync((Secretary)worker),
                _ => throw new ArgumentException("Invalid role")
            };
        }

        public async Task<Worker> GetByEmailAsync(string email)
        {
            var worker = await _workerRepository.GetByEmailAsync(email);
            if (worker == null)
            {
                throw new KeyNotFoundException($"User with email {email} not found.");
            }
            return worker;
        }

        private async Task<int?> ResolveAddressAsync(CreateWorkerRequest request)
        {
            if (request.AddressId.HasValue)
            {
                var existingAddress = await _addressRepository.GetByIdAsync(request.AddressId.Value);
                if (existingAddress == null)
                {
                    throw new ArgumentException($"Address with ID {request.AddressId.Value} not found");
                }
                return request.AddressId.Value;
            }

            if (request.NewAddress != null)
            {
                var city = await _cityRepository.GetByIdAsync(request.NewAddress.CityId);
                if (city == null)
                {
                    throw new ArgumentException($"City with ID {request.NewAddress.CityId} not found");
                }

                var address = new Address
                {
                    StreetName = request.NewAddress.StreetName,
                    StreetNumber = request.NewAddress.StreetNumber,
                    CityId = request.NewAddress.CityId,
                    ApartmentNumber = request.NewAddress.ApartmentNumber,
                    AdditionalInfo = request.NewAddress.AdditionalInfo
                };

                var createdAddress = await _addressRepository.CreateAsync(address);
                return createdAddress.Id;
            }

            return null;
        }
    }
}
