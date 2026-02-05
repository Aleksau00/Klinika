using Klinika.DATA;
using Klinika.Models;
using Microsoft.EntityFrameworkCore;

namespace Klinika.Repositories
{
    public class AllergenRepository : IAllergenRepository
    {
        private readonly ApplicationDbContext _context;

        public AllergenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Allergen?> GetByIdAsync(int id)
        {
            return await _context.Allergens.FindAsync(id);
        }

        public async Task<Allergen?> GetByNameAsync(string name)
        {
            return await _context.Allergens
                .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Allergen>> GetAllAsync()
        {
            return await _context.Allergens
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<Allergen> CreateAsync(Allergen allergen)
        {
            _context.Allergens.Add(allergen);
            await _context.SaveChangesAsync();
            return allergen;
        }

        public async Task<Allergen> UpdateAsync(Allergen allergen)
        {
            _context.Allergens.Update(allergen);
            await _context.SaveChangesAsync();
            return allergen;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var allergen = await GetByIdAsync(id);
            if (allergen == null)
                return false;

            _context.Allergens.Remove(allergen);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PatientAllergen>> GetPatientAllergensAsync(int patientId)
        {
            return await _context.PatientAllergens
                .Include(pa => pa.Allergen)
                .Include(pa => pa.Patient)
                .Where(pa => pa.PatientId == patientId)
                .OrderBy(pa => pa.Allergen.Name)
                .ToListAsync();
        }

        public async Task<PatientAllergen?> GetPatientAllergenAsync(int patientId, int allergenId)
        {
            return await _context.PatientAllergens
                .Include(pa => pa.Allergen)
                .Include(pa => pa.Patient)
                .FirstOrDefaultAsync(pa => pa.PatientId == patientId && pa.AllergenId == allergenId);
        }

        public async Task<PatientAllergen> AddPatientAllergenAsync(PatientAllergen patientAllergen)
        {
            _context.PatientAllergens.Add(patientAllergen);
            await _context.SaveChangesAsync();
            
            // Load navigation properties
            await _context.Entry(patientAllergen).Reference(pa => pa.Allergen).LoadAsync();
            await _context.Entry(patientAllergen).Reference(pa => pa.Patient).LoadAsync();
            
            return patientAllergen;
        }

        public async Task<PatientAllergen> UpdatePatientAllergenAsync(PatientAllergen patientAllergen)
        {
            _context.PatientAllergens.Update(patientAllergen);
            await _context.SaveChangesAsync();
            
            await _context.Entry(patientAllergen).Reference(pa => pa.Allergen).LoadAsync();
            await _context.Entry(patientAllergen).Reference(pa => pa.Patient).LoadAsync();
            
            return patientAllergen;
        }

        public async Task<bool> RemovePatientAllergenAsync(int patientId, int allergenId)
        {
            var patientAllergen = await _context.PatientAllergens
                .FirstOrDefaultAsync(pa => pa.PatientId == patientId && pa.AllergenId == allergenId);
            
            if (patientAllergen == null)
                return false;

            _context.PatientAllergens.Remove(patientAllergen);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}