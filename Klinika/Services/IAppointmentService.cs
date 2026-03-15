using Klinika.Models.DTOs;

namespace Klinika.Services
{
    public interface IAppointmentService
    {
        // Booking
        Task<AppointmentDto> BookAppointmentAsync(CreateAppointmentRequest request, int secretaryId);

        // Retrieval
        Task<AppointmentDto?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(int patientId);
        Task<IEnumerable<AppointmentDto>> GetDoctorScheduleAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null);
        Task<IEnumerable<AppointmentDto>> GetClinicScheduleAsync(int clinicId, DateOnly date);

        // Lifecycle Management (Secretary)
        Task<AppointmentDto> CheckInPatientAsync(int appointmentId);
        Task<bool> CancelAppointmentAsync(int appointmentId, string reason);
        Task<bool> MarkNoShowAsync(int appointmentId);

        // Doctor Actions
        Task<AppointmentDto> CompleteTreatmentAppointmentAsync(int id, CompleteTreatmentRequest request);
        Task<AppointmentDto> CompletePreventiveAppointmentAsync(int id, CompletePreventiveRequest request);
    }
}