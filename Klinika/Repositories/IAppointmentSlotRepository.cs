using Klinika.Models;

namespace Klinika.Repositories
{
    public interface IAppointmentSlotRepository
    {
        Task<AppointmentSlot> GetByIdAsync(int id);
        Task<IEnumerable<AppointmentSlot>> GetSlotsByDoctorAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null);
        Task<IEnumerable<AppointmentSlot>> GetAvailableSlotsByDoctorAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null);
        Task<AppointmentSlot> CreateSlotAsync(AppointmentSlot slot);
        Task<IEnumerable<AppointmentSlot>> CreateSlotsAsync(IEnumerable<AppointmentSlot> slots);
        Task<AppointmentSlot> UpdateSlotAsync(AppointmentSlot slot);
        Task<bool> DeleteSlotAsync(int id);
        Task<bool> SlotExistsAsync(int doctorId, DateOnly date, TimeOnly startTime);
    }
}