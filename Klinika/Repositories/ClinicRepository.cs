using Klinika.DATA;
using Klinika.Models;
using Microsoft.EntityFrameworkCore;

namespace Klinika.Repositories
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly ApplicationDbContext _context;

        public ClinicRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Clinic>> GetAllAsync()
        {
            return await _context.Clinics
                .Include(c => c.Address)
                    .ThenInclude(a => a.City)
                .Include(c => c.Workers)
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Clinic?> GetByIdAsync(int id)
        {
            return await _context.Clinics
                .Include(c => c.Address)
                    .ThenInclude(a => a.City)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Clinic?> GetByIdWithWorkersAsync(int id)
        {
            return await _context.Clinics
                .Include(c => c.Address)
                    .ThenInclude(a => a.City)
                .Include(c => c.Workers)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Clinic> CreateAsync(Clinic clinic)
        {
            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();
            
            await _context.Entry(clinic).Reference(c => c.Address).LoadAsync();
            await _context.Entry(clinic.Address).Reference(a => a.City).LoadAsync();
            
            return clinic;
        }

        public async Task<Clinic> UpdateAsync(Clinic clinic)
        {
            _context.Clinics.Update(clinic);
            await _context.SaveChangesAsync();
            
            await _context.Entry(clinic).Reference(c => c.Address).LoadAsync();
            await _context.Entry(clinic.Address).Reference(a => a.City).LoadAsync();
            
            return clinic;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return false;

            clinic.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}