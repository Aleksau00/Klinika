using Klinika.Models.DTOs;

namespace Klinika.Services
{
    public interface IClinicService
    {
        Task<IEnumerable<ClinicDto>> GetAllClinicsAsync();
        Task<ClinicDto?> GetClinicByIdAsync(int id);
        Task<IEnumerable<WorkerDto>> GetClinicWorkersAsync(int clinicId);
        Task<ClinicDto> CreateClinicAsync(CreateClinicRequest request);
        Task<ClinicDto> UpdateClinicAsync(int id, UpdateClinicRequest request);
        Task<bool> DeleteClinicAsync(int id);
    }
}