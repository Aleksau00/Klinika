using Klinika.Models;
using Klinika.Models.DTOs;
using Klinika.Repositories;

namespace Klinika.Services
{
    public class AllergenService : IAllergenService
    {
        private readonly IAllergenRepository _allergenRepository;

        public AllergenService(IAllergenRepository allergenRepository)
        {
            _allergenRepository = allergenRepository;
        }

        public async Task<AllergenDto?> GetAllergenByIdAsync(int id)
        {
            var allergen = await _allergenRepository.GetByIdAsync(id);
            return allergen == null ? null : MapToAllergenDto(allergen);
        }

        public async Task<IEnumerable<AllergenDto>> GetAllAllergensAsync()
        {
            var allergens = await _allergenRepository.GetAllAsync();
            return allergens.Select(MapToAllergenDto);
        }

        public async Task<AllergenDto> CreateAllergenAsync(CreateAllergenRequest request)
        {
            // Check if allergen already exists
            var existing = await _allergenRepository.GetByNameAsync(request.Name);
            if (existing != null)
                throw new InvalidOperationException($"Allergen with name '{request.Name}' already exists");

            var allergen = new Allergen
            {
                Name = request.Name,
                Description = request.Description
            };

            var created = await _allergenRepository.CreateAsync(allergen);
            return MapToAllergenDto(created);
        }

        public async Task<AllergenDto> UpdateAllergenAsync(int id, CreateAllergenRequest request)
        {
            var allergen = await _allergenRepository.GetByIdAsync(id);
            if (allergen == null)
                throw new KeyNotFoundException("Allergen not found");

            allergen.Name = request.Name;
            allergen.Description = request.Description;

            var updated = await _allergenRepository.UpdateAsync(allergen);
            return MapToAllergenDto(updated);
        }

        public async Task<bool> DeleteAllergenAsync(int id)
        {
            return await _allergenRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<PatientAllergenDto>> GetPatientAllergensAsync(int patientId)
        {
            var patientAllergens = await _allergenRepository.GetPatientAllergensAsync(patientId);
            return patientAllergens.Select(MapToPatientAllergenDto);
        }

        public async Task<PatientAllergenDto> AddAllergenToPatientAsync(AddPatientAllergenRequest request)
        {
            // Check if allergen exists
            var allergen = await _allergenRepository.GetByIdAsync(request.AllergenId);
            if (allergen == null)
                throw new KeyNotFoundException("Allergen not found");

            // Check if patient already has this allergen
            var existing = await _allergenRepository.GetPatientAllergenAsync(request.PatientId, request.AllergenId);
            if (existing != null)
                throw new InvalidOperationException("Patient already has this allergen");

            var patientAllergen = new PatientAllergen
            {
                PatientId = request.PatientId,
                AllergenId = request.AllergenId,
                DiagnosedDate = request.DiagnosedDate,
                Notes = request.Notes
            };

            var created = await _allergenRepository.AddPatientAllergenAsync(patientAllergen);
            return MapToPatientAllergenDto(created);
        }

        public async Task<PatientAllergenDto> UpdatePatientAllergenAsync(int patientId, int allergenId, UpdatePatientAllergenRequest request)
        {
            var patientAllergen = await _allergenRepository.GetPatientAllergenAsync(patientId, allergenId);
            if (patientAllergen == null)
                throw new KeyNotFoundException("Patient allergen association not found");

            patientAllergen.DiagnosedDate = request.DiagnosedDate;
            patientAllergen.Notes = request.Notes;

            var updated = await _allergenRepository.UpdatePatientAllergenAsync(patientAllergen);
            return MapToPatientAllergenDto(updated);
        }

        public async Task<bool> RemoveAllergenFromPatientAsync(int patientId, int allergenId)
        {
            return await _allergenRepository.RemovePatientAllergenAsync(patientId, allergenId);
        }

        private static AllergenDto MapToAllergenDto(Allergen allergen)
        {
            return new AllergenDto
            {
                Id = allergen.Id,
                Name = allergen.Name,
                Description = allergen.Description
            };
        }

        private static PatientAllergenDto MapToPatientAllergenDto(PatientAllergen pa)
        {
            return new PatientAllergenDto
            {
                AllergenId = pa.AllergenId,
                AllergenName = pa.Allergen.Name,
                AllergenDescription = pa.Allergen.Description,
                PatientId = pa.PatientId,
                PatientName = $"{pa.Patient.FirstName} {pa.Patient.LastName}",
                DiagnosedDate = pa.DiagnosedDate,
                Notes = pa.Notes
            };
        }
    }
}