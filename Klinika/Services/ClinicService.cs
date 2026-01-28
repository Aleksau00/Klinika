using Klinika.Models.DTOs;
using Klinika.Models;
using Klinika.Repositories;

namespace Klinika.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICityRepository _cityRepository;

        public ClinicService(
            IClinicRepository clinicRepository,
            IAddressRepository addressRepository,
            ICityRepository cityRepository)
        {
            _clinicRepository = clinicRepository;
            _addressRepository = addressRepository;
            _cityRepository = cityRepository;
        }

        public async Task<IEnumerable<ClinicDto>> GetAllClinicsAsync()
        {
            var clinics = await _clinicRepository.GetAllAsync();
            return clinics.Select(MapToDto);
        }

        public async Task<ClinicDto?> GetClinicByIdAsync(int id)
        {
            var clinic = await _clinicRepository.GetByIdAsync(id);
            return clinic != null ? MapToDto(clinic) : null;
        }

        public async Task<IEnumerable<WorkerDto>> GetClinicWorkersAsync(int clinicId)
        {
            var clinic = await _clinicRepository.GetByIdWithWorkersAsync(clinicId);
            if (clinic == null)
                throw new KeyNotFoundException($"Clinic with ID {clinicId} not found");

            return clinic.Workers.Select(MapWorkerToDto);
        }

        public async Task<ClinicDto> CreateClinicAsync(CreateClinicRequest request)
        {
            int addressId = await ResolveAddressAsync(request.AddressId, request.NewAddress);

            var clinic = new Clinic
            {
                Name = request.Name,
                AddressId = addressId,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _clinicRepository.CreateAsync(clinic);
            return MapToDto(created);
        }

        public async Task<ClinicDto> UpdateClinicAsync(int id, UpdateClinicRequest request)
        {
            var clinic = await _clinicRepository.GetByIdAsync(id);
            if (clinic == null)
                throw new KeyNotFoundException($"Clinic with ID {id} not found");

            clinic.Name = request.Name;
            clinic.AddressId = request.AddressId;
            clinic.PhoneNumber = request.PhoneNumber;
            clinic.Email = request.Email;
            clinic.IsActive = request.IsActive;

            var updated = await _clinicRepository.UpdateAsync(clinic);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteClinicAsync(int id)
        {
            return await _clinicRepository.DeleteAsync(id);
        }

        private ClinicDto MapToDto(Clinic clinic)
        {
            return new ClinicDto
            {
                Id = clinic.Id,
                Name = clinic.Name,
                Address = new AddressDto
                {
                    Id = clinic.Address.Id,
                    StreetName = clinic.Address.StreetName,
                    StreetNumber = clinic.Address.StreetNumber,
                    ApartmentNumber = clinic.Address.ApartmentNumber,
                    AdditionalInfo = clinic.Address.AdditionalInfo,
                    City = new CityDto
                    {
                        Id = clinic.Address.City.Id,
                        Name = clinic.Address.City.Name,
                        PostalCode = clinic.Address.City.PostalCode,
                        Country = clinic.Address.City.Country
                    }
                },
                PhoneNumber = clinic.PhoneNumber,
                Email = clinic.Email,
                IsActive = clinic.IsActive,
                CreatedAt = clinic.CreatedAt,
                WorkerCount = clinic.Workers?.Count ?? 0
            };
        }

        private WorkerDto MapWorkerToDto(Worker worker)
        {
            var dto = new WorkerDto
            {
                Id = worker.Id,
                FirstName = worker.FirstName,
                LastName = worker.LastName,
                Email = worker.Email,
                PhoneNumber = worker.PhoneNumber,
                ClinicId = worker.ClinicId,
                ClinicName = worker.Clinic?.Name,
                IsActive = worker.IsActive,
                Role = worker switch
                {
                    Administrator => "Administrator",
                    Doctor => "Doctor",
                    Secretary => "Secretary",
                    _ => "Worker"
                }
            };

            // Add role-specific fields
            if (worker is Doctor doctor)
            {
                dto.Specialty = doctor.Specialty;
                dto.LicenseNumber = doctor.LicenseNumber;
            }
            else if (worker is Secretary secretary)
            {
                dto.Qualification = secretary.Qualification;
            }
            else if (worker is Administrator admin)
            {
                dto.SeniorityLevel = admin.SeniorityLevel;
            }

            return dto;
        }

        private async Task<int> ResolveAddressAsync(int? addressId, CreateAddressRequest? newAddress)
        {
            if (addressId.HasValue)
            {
                var existing = await _addressRepository.GetByIdAsync(addressId.Value);
                if (existing == null)
                    throw new ArgumentException($"Address with ID {addressId.Value} not found");
                return addressId.Value;
            }

            if (newAddress != null)
            {
                var city = await _cityRepository.GetByIdAsync(newAddress.CityId);
                if (city == null)
                    throw new ArgumentException($"City with ID {newAddress.CityId} not found");

                var address = new Address
                {
                    StreetName = newAddress.StreetName,
                    StreetNumber = newAddress.StreetNumber,
                    CityId = newAddress.CityId,
                    ApartmentNumber = newAddress.ApartmentNumber,
                    AdditionalInfo = newAddress.AdditionalInfo
                };

                var created = await _addressRepository.CreateAsync(address);
                return created.Id;
            }

            throw new ArgumentException("Either AddressId or NewAddress must be provided");
        }
    }
}