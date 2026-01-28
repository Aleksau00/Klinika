using Klinika.Models;
using Klinika.Models.DTOs;

namespace Klinika.Services
{
    public interface IWorkerService
    {
        Task<Worker> GetByIdAsync(int id);
        Task<Worker> GetByEmailAsync(string email);
        Task<Worker> CreateAsync(CreateWorkerRequest request);
    }
}
