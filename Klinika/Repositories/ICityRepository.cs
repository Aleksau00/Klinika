using Klinika.Models;

namespace Klinika.Repositories
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<City?> GetByIdAsync(int id);
        Task<City?> GetByNameAsync(string name, string country);
        Task<City> CreateAsync(City city);
    }
}