using Klinika.DATA;
using Klinika.Models;
using Microsoft.EntityFrameworkCore;

namespace Klinika.Repositories
{
    public class VaccinationRepository : IVaccinationRepository
    {
        private readonly ApplicationDbContext _context;

        public VaccinationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Vaccination?> GetByIdAsync(int id)
        {
            return await _context.Vaccinations.FindAsync(id);
        }

        public async Task<Vaccination?> GetByNameAsync(string name)
        {
            return await _context.Vaccinations
                .FirstOrDefaultAsync(v => v.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Vaccination>> GetAllAsync()
        {
            return await _context.Vaccinations
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task<Vaccination> CreateAsync(Vaccination vaccination)
        {
            _context.Vaccinations.Add(vaccination);
            await _context.SaveChangesAsync();
            return vaccination;
        }

        public async Task<Vaccination> UpdateAsync(Vaccination vaccination)
        {
            _context.Vaccinations.Update(vaccination);
            await _context.SaveChangesAsync();
            return vaccination;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vaccination = await GetByIdAsync(id);
            if (vaccination == null)
                return false;

            _context.Vaccinations.Remove(vaccination);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<VaccinationRecord?> GetRecordByIdAsync(int id)
        {
            return await _context.VaccinationRecords
                .Include(vr => vr.Patient)
                .Include(vr => vr.Vaccination)
                .Include(vr => vr.AdministeredByDoctor)
                .FirstOrDefaultAsync(vr => vr.Id == id);
        }

        public async Task<IEnumerable<VaccinationRecord>> GetPatientVaccinationRecordsAsync(int patientId)
        {
            return await _context.VaccinationRecords
                .Include(vr => vr.Vaccination)
                .Include(vr => vr.AdministeredByDoctor)
                .Where(vr => vr.PatientId == patientId)
                .OrderByDescending(vr => vr.AdministeredDate)
                .ToListAsync();
        }

        public async Task<VaccinationRecord> CreateRecordAsync(VaccinationRecord record)
        {
            _context.VaccinationRecords.Add(record);
            await _context.SaveChangesAsync();
            
            await _context.Entry(record).Reference(vr => vr.Patient).LoadAsync();
            await _context.Entry(record).Reference(vr => vr.Vaccination).LoadAsync();
            await _context.Entry(record).Reference(vr => vr.AdministeredByDoctor).LoadAsync();
            
            return record;
        }

        public async Task<VaccinationRecord> UpdateRecordAsync(VaccinationRecord record)
        {
            _context.VaccinationRecords.Update(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task<bool> DeleteRecordAsync(int id)
        {
            var record = await _context.VaccinationRecords.FindAsync(id);
            if (record == null)
                return false;

            _context.VaccinationRecords.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}