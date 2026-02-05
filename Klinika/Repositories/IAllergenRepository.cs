using Klinika.Models;

namespace Klinika.Repositories
{
    public interface IAllergenRepository
    {
        // Allergen management
        Task<Allergen?> GetByIdAsync(int id);
        Task<Allergen?> GetByNameAsync(string name);
        Task<IEnumerable<Allergen>> GetAllAsync();
        Task<Allergen> CreateAsync(Allergen allergen);
        Task<Allergen> UpdateAsync(Allergen allergen);
        Task<bool> DeleteAsync(int id);
        
        // Patient-Allergen relationship management
        Task<IEnumerable<PatientAllergen>> GetPatientAllergensAsync(int patientId);
        Task<PatientAllergen?> GetPatientAllergenAsync(int patientId, int allergenId);
        Task<PatientAllergen> AddPatientAllergenAsync(PatientAllergen patientAllergen);
        Task<PatientAllergen> UpdatePatientAllergenAsync(PatientAllergen patientAllergen);
        Task<bool> RemovePatientAllergenAsync(int patientId, int allergenId);
    }
}