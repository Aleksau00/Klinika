using Klinika.DATA;
using Klinika.Models;
using Microsoft.EntityFrameworkCore;

namespace Klinika.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments.FindAsync(id);
        }

        public async Task<Appointment?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                    .ThenInclude(c => c.Address)
                        .ThenInclude(a => a.City)
                .Include(a => a.BookedByWorker)
                .Include(a => a.AppointmentSlot)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .Include(a => a.BookedByWorker)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.ScheduledDate)
                .ThenByDescending(a => a.ScheduledStartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .Include(a => a.BookedByWorker)
                .Where(a => a.DoctorId == doctorId);

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.ScheduledDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.ScheduledDate <= toDate.Value);
            }

            return await query
                .OrderBy(a => a.ScheduledDate)
                .ThenBy(a => a.ScheduledStartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByClinicIdAsync(int clinicId, DateOnly date)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .Include(a => a.BookedByWorker)
                .Where(a => a.ClinicId == clinicId && a.ScheduledDate == date)
                .OrderBy(a => a.ScheduledStartTime)
                .ToListAsync();
        }

        public async Task<TreatmentAppointment> CreateTreatmentAsync(TreatmentAppointment appointment)
        {
            _context.TreatmentAppointments.Add(appointment);
            await _context.SaveChangesAsync();
            
            // Load navigation properties
            await _context.Entry(appointment).Reference(a => a.Patient).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Doctor).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Clinic).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.BookedByWorker).LoadAsync();
            
            return appointment;
        }

        public async Task<PreventiveAppointment> CreatePreventiveAsync(PreventiveAppointment appointment)
        {
            _context.PreventiveAppointments.Add(appointment);
            await _context.SaveChangesAsync();
            
            // Load navigation properties
            await _context.Entry(appointment).Reference(a => a.Patient).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Doctor).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Clinic).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.BookedByWorker).LoadAsync();
            
            return appointment;
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<bool> IsSlotBookedAsync(int slotId)
        {
            return await _context.Appointments
                .AnyAsync(a => a.AppointmentSlotId == slotId && 
                              a.Status != AppointmentStatus.Cancelled && 
                              a.Status != AppointmentStatus.NoShow);
        }

        public async Task<int> GetPatientNoShowCountAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            return patient?.NoShowCount ?? 0;
        }
    }
}