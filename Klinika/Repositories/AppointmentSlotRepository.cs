using Klinika.DATA;
using Klinika.Models;
using Microsoft.EntityFrameworkCore;

namespace Klinika.Repositories
{
    public class AppointmentSlotRepository : IAppointmentSlotRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentSlotRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentSlot> GetByIdAsync(int id)
        {
            return await _context.AppointmentSlots
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<AppointmentSlot>> GetSlotsByDoctorAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            var query = _context.AppointmentSlots
                .Include(s => s.Doctor)
                .Where(s => s.DoctorId == doctorId);

            if (fromDate.HasValue)
                query = query.Where(s => s.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.Date <= toDate.Value);

            return await query.OrderBy(s => s.Date).ThenBy(s => s.StartTime).ToListAsync();
        }

        public async Task<IEnumerable<AppointmentSlot>> GetAvailableSlotsByDoctorAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            var query = _context.AppointmentSlots
                .Include(s => s.Doctor)
                .Where(s => s.DoctorId == doctorId && s.IsAvailable);

            if (fromDate.HasValue)
                query = query.Where(s => s.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.Date <= toDate.Value);

            return await query.OrderBy(s => s.Date).ThenBy(s => s.StartTime).ToListAsync();
        }

        public async Task<AppointmentSlot> CreateSlotAsync(AppointmentSlot slot)
        {
            _context.AppointmentSlots.Add(slot);
            await _context.SaveChangesAsync();
            return slot;
        }

        public async Task<IEnumerable<AppointmentSlot>> CreateSlotsAsync(IEnumerable<AppointmentSlot> slots)
        {
            _context.AppointmentSlots.AddRange(slots);
            await _context.SaveChangesAsync();
            return slots;
        }

        public async Task<AppointmentSlot> UpdateSlotAsync(AppointmentSlot slot)
        {
            _context.AppointmentSlots.Update(slot);
            await _context.SaveChangesAsync();
            return slot;
        }

        public async Task<bool> DeleteSlotAsync(int id)
        {
            var slot = await _context.AppointmentSlots.FindAsync(id);
            if (slot == null)
                return false;

            _context.AppointmentSlots.Remove(slot);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SlotExistsAsync(int doctorId, DateOnly date, TimeOnly startTime)
        {
            return await _context.AppointmentSlots
                .AnyAsync(s => s.DoctorId == doctorId && s.Date == date && s.StartTime == startTime);
        }
    }
}