using Klinika.DATA;
using Klinika.Models;
using Microsoft.EntityFrameworkCore;

namespace Klinika.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly ApplicationDbContext _context;

        public CityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _context.Cities
                .OrderBy(c => c.Country)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<City?> GetByIdAsync(int id)
        {
            return await _context.Cities.FindAsync(id);
        }

        public async Task<City?> GetByNameAsync(string name, string country)
        {
            return await _context.Cities
                .FirstOrDefaultAsync(c => c.Name == name && c.Country == country);
        }

        public async Task<City> CreateAsync(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
            return city;
        }
    }
}