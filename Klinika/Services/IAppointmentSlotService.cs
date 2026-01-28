using Klinika.Models;
using Klinika.Models.DTOs;

namespace Klinika.Services
{
    public interface IAppointmentSlotService
    {
        Task<IEnumerable<AppointmentSlotResponse>> GetDoctorSlotsAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null);
        Task<IEnumerable<AppointmentSlotResponse>> GetAvailableSlotsAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null);
        Task<AppointmentSlotResponse> CreateSlotAsync(int doctorId, DateOnly date, TimeOnly startTime);
        Task<IEnumerable<AppointmentSlotResponse>> CreateWeeklySlotsAsync(CreateWeeklySlotsRequest request);
        Task<IEnumerable<AppointmentSlotResponse>> CreateCustomSlotsAsync(int doctorId, CreateCustomSlotsRequest request); // NEW
        Task<AppointmentSlotResponse> MarkSlotAsUnavailableAsync(int slotId);
        Task<bool> DeleteSlotAsync(int slotId);
    }
}   