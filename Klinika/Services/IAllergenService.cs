using Klinika.Models.DTOs;

namespace Klinika.Services
{
    public interface IAllergenService
    {
        // Allergen management
        Task<AllergenDto?> GetAllergenByIdAsync(int id);
        Task<IEnumerable<AllergenDto>> GetAllAllergensAsync();
        Task<AllergenDto> CreateAllergenAsync(CreateAllergenRequest request);
        Task<AllergenDto> UpdateAllergenAsync(int id, CreateAllergenRequest request);
        Task<bool> DeleteAllergenAsync(int id);
        
        // Patient-Allergen relationship management
        Task<IEnumerable<PatientAllergenDto>> GetPatientAllergensAsync(int patientId);
        Task<PatientAllergenDto> AddAllergenToPatientAsync(AddPatientAllergenRequest request);
        Task<PatientAllergenDto> UpdatePatientAllergenAsync(int patientId, int allergenId, UpdatePatientAllergenRequest request);
        Task<bool> RemoveAllergenFromPatientAsync(int patientId, int allergenId);
    }
}