using Klinika.Models.DTOs;

namespace Klinika.Services
{
    public interface IVaccinationService
    {
        // Vaccination management
        Task<VaccinationDto?> GetVaccinationByIdAsync(int id);
        Task<IEnumerable<VaccinationDto>> GetAllVaccinationsAsync();
        Task<VaccinationDto> CreateVaccinationAsync(CreateVaccinationRequest request);
        Task<VaccinationDto> UpdateVaccinationAsync(int id, CreateVaccinationRequest request);
        Task<bool> DeleteVaccinationAsync(int id);
        
        // Vaccination records
        Task<VaccinationRecordDto?> GetVaccinationRecordByIdAsync(int id);
        Task<IEnumerable<VaccinationRecordDto>> GetPatientVaccinationRecordsAsync(int patientId);
        Task<VaccinationRecordDto> AdministerVaccinationAsync(AdministerVaccinationRequest request, int doctorId);
        Task<bool> DeleteVaccinationRecordAsync(int id);
    }
}