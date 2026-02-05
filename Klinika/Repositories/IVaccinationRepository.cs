using Klinika.Models;

namespace Klinika.Repositories
{
    public interface IVaccinationRepository
    {
        // Vaccination management
        Task<Vaccination?> GetByIdAsync(int id);
        Task<Vaccination?> GetByNameAsync(string name);
        Task<IEnumerable<Vaccination>> GetAllAsync();
        Task<Vaccination> CreateAsync(Vaccination vaccination);
        Task<Vaccination> UpdateAsync(Vaccination vaccination);
        Task<bool> DeleteAsync(int id);
        
        // Vaccination records
        Task<VaccinationRecord?> GetRecordByIdAsync(int id);
        Task<IEnumerable<VaccinationRecord>> GetPatientVaccinationRecordsAsync(int patientId);
        Task<VaccinationRecord> CreateRecordAsync(VaccinationRecord record);
        Task<VaccinationRecord> UpdateRecordAsync(VaccinationRecord record);
        Task<bool> DeleteRecordAsync(int id);
    }
}