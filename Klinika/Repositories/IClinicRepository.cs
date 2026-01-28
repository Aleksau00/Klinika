using Klinika.Models;

namespace Klinika.Repositories
{
    public interface IClinicRepository
    {
        Task<IEnumerable<Clinic>> GetAllAsync();
        Task<Clinic?> GetByIdAsync(int id);
        Task<Clinic?> GetByIdWithWorkersAsync(int id);
        Task<Clinic> CreateAsync(Clinic clinic);
        Task<Clinic> UpdateAsync(Clinic clinic);
        Task<bool> DeleteAsync(int id);
    }
}