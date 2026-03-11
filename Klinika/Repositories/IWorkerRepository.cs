using Klinika.Models;

public interface IWorkerRepository
{
    Task<List<Worker>> GetAllAsync();
    Task<Worker> GetByIdAsync(int id);
    Task<Worker> GetByEmailAsync(string email);

    Task<Administrator> CreateAdministratorAsync(Administrator admin);
    Task<Doctor> CreateDoctorAsync(Doctor doctor);
    Task<Secretary> CreateSecretaryAsync(Secretary secretary);

    Task<Worker> UpdateAsync(Worker worker);
}