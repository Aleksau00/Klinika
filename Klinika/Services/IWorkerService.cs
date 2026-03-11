using Klinika.Models;
using Klinika.Models.DTOs;

namespace Klinika.Services
{
    public interface IWorkerService
    {
        Task<List<Worker>> GetAllAsync();
        Task<Worker> GetByIdAsync(int id);
        Task<Worker> GetByEmailAsync(string email);
        Task<Worker> CreateAsync(CreateWorkerRequest request);
        Task<Worker> UpdateAsync(int id, UpdateWorkerRequest request);
        Task SetActiveAsync(int id, bool isActive);
    }
}
