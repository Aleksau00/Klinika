using Klinika.Models;

namespace Klinika.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null);
        Task<IEnumerable<Appointment>> GetByClinicIdAsync(int clinicId, DateOnly date);
        Task<TreatmentAppointment> CreateTreatmentAsync(TreatmentAppointment appointment);
        Task<PreventiveAppointment> CreatePreventiveAsync(PreventiveAppointment appointment);
        Task<Appointment> UpdateAsync(Appointment appointment);
        Task<bool> IsSlotBookedAsync(int slotId);
        Task<int> GetPatientNoShowCountAsync(int patientId);
    }
}
