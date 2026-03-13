using Klinika.DATA;
using Klinika.Models;
using Klinika.Models.DTOs;
using Klinika.Repositories;

namespace Klinika.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAppointmentSlotRepository _slotRepository;
        private readonly ApplicationDbContext _context;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IAppointmentSlotRepository slotRepository,
            ApplicationDbContext context)
        {
            _appointmentRepository = appointmentRepository;
            _slotRepository = slotRepository;
            _context = context;
        }

        public async Task<AppointmentDto> BookAppointmentAsync(CreateAppointmentRequest request, int secretaryId)
        {
            // Check if slot exists and is available
            var slot = await _slotRepository.GetByIdAsync(request.AppointmentSlotId);
            if (slot == null)
                throw new KeyNotFoundException($"Appointment slot with ID {request.AppointmentSlotId} not found");

            if (!slot.IsAvailable)
                throw new InvalidOperationException("This time slot is no longer available");

            // Check if slot is already booked
            if (await _appointmentRepository.IsSlotBookedAsync(request.AppointmentSlotId))
                throw new InvalidOperationException("This time slot is already booked");

            // Check if date is in the past
            if (slot.Date < DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidOperationException("Cannot book appointments in the past");

            // Create appropriate appointment type
            Appointment appointment;
            
            if (request.AppointmentType == AppointmentType.Treatment)
            {
                appointment = new TreatmentAppointment
                {
                    AppointmentSlotId = request.AppointmentSlotId,
                    PatientId = request.PatientId,
                    ClinicId = slot.Doctor.ClinicId ?? throw new InvalidOperationException("Doctor must be assigned to a clinic"),
                    DoctorId = slot.DoctorId,
                    Status = AppointmentStatus.Scheduled,
                    ScheduledDate = slot.Date,
                    ScheduledStartTime = slot.StartTime,
                    ScheduledEndTime = slot.EndTime,
                    BookedByWorkerId = secretaryId,
                    BookedAt = DateTime.UtcNow
                };
                
                appointment = await _appointmentRepository.CreateTreatmentAsync((TreatmentAppointment)appointment);
            }
            else
            {
                appointment = new PreventiveAppointment
                {
                    AppointmentSlotId = request.AppointmentSlotId,
                    PatientId = request.PatientId,
                    ClinicId = slot.Doctor.ClinicId ?? throw new InvalidOperationException("Doctor must be assigned to a clinic"),
                    DoctorId = slot.DoctorId,
                    Status = AppointmentStatus.Scheduled,
                    ScheduledDate = slot.Date,
                    ScheduledStartTime = slot.StartTime,
                    ScheduledEndTime = slot.EndTime,
                    BookedByWorkerId = secretaryId,
                    BookedAt = DateTime.UtcNow
                };
                
                appointment = await _appointmentRepository.CreatePreventiveAsync((PreventiveAppointment)appointment);
            }

            // Mark slot as unavailable
            slot.IsAvailable = false;
            await _context.SaveChangesAsync();

            return await MapToDtoAsync(appointment);
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(id);
            return appointment != null ? await MapToDtoAsync(appointment) : null;
        }

        public async Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(int patientId)
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
            var dtos = new List<AppointmentDto>();
            foreach (var appointment in appointments)
            {
                dtos.Add(await MapToDtoAsync(appointment));
            }
            return dtos;
        }

        public async Task<IEnumerable<AppointmentDto>> GetDoctorScheduleAsync(int doctorId, DateOnly? date = null)
        {
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId, date);
            var dtos = new List<AppointmentDto>();
            foreach (var appointment in appointments)
            {
                dtos.Add(await MapToDtoAsync(appointment));
            }
            return dtos;
        }

        public async Task<IEnumerable<AppointmentDto>> GetClinicScheduleAsync(int clinicId, DateOnly date)
        {
            var appointments = await _appointmentRepository.GetByClinicIdAsync(clinicId, date);
            var dtos = new List<AppointmentDto>();
            foreach (var appointment in appointments)
            {
                dtos.Add(await MapToDtoAsync(appointment));
            }
            return dtos;
        }

        public async Task<AppointmentDto> CheckInPatientAsync(int appointmentId)
        {
            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found");

            if (appointment.Status != AppointmentStatus.Scheduled)
                throw new InvalidOperationException($"Cannot check in appointment with status: {appointment.Status}");

            appointment.Status = AppointmentStatus.InProgress;
            appointment.CheckedInAt = DateTime.UtcNow;
            
            await _appointmentRepository.UpdateAsync(appointment);
            return await MapToDtoAsync(appointment);
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId, string reason)
        {
            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found");

            if (appointment.Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed appointment");

            if (appointment.Status == AppointmentStatus.Cancelled)
                return true; // make DELETE idempotent instead of throwing

            // 48-hour cancellation rule
            var appointmentDateTime = appointment.ScheduledDate.ToDateTime(appointment.ScheduledStartTime);
            var hoursUntilAppointment = (appointmentDateTime - DateTime.Now).TotalHours;
            
            if (hoursUntilAppointment < 48)
                throw new InvalidOperationException("Cannot cancel appointment less than 48 hours before scheduled time");

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancelledAt = DateTime.UtcNow;
            appointment.CancellationReason = reason;
            
            // Free up the slot
            var slot = await _slotRepository.GetByIdAsync(appointment.AppointmentSlotId);
            if (slot != null)
            {
                slot.IsAvailable = true;
                await _context.SaveChangesAsync();
            }
            
            await _appointmentRepository.UpdateAsync(appointment);
            return true;
        }

        public async Task<bool> MarkNoShowAsync(int appointmentId)
        {
            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found");

            if (appointment.Status != AppointmentStatus.Scheduled && appointment.Status != AppointmentStatus.InProgress)
                throw new InvalidOperationException($"Cannot mark no-show for appointment with status: {appointment.Status}");

            appointment.Status = AppointmentStatus.NoShow;
            
            // Increment patient no-show count
            var patient = await _context.Patients.FindAsync(appointment.PatientId);
            if (patient != null)
            {
                patient.NoShowCount++;
            }
            
            // Free up the slot
            var slot = await _slotRepository.GetByIdAsync(appointment.AppointmentSlotId);
            if (slot != null)
            {
                slot.IsAvailable = true;
            }
            
            await _context.SaveChangesAsync();
            await _appointmentRepository.UpdateAsync(appointment);
            return true;
        }

        public async Task<AppointmentDto> CompleteTreatmentAppointmentAsync(int id, CompleteTreatmentRequest request)
        {
            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found");

            if (appointment is not TreatmentAppointment treatmentAppointment)
                throw new InvalidOperationException("Appointment is not a treatment appointment");

            if (appointment.Status != AppointmentStatus.InProgress)
                throw new InvalidOperationException($"Cannot complete appointment with status: {appointment.Status}");

            treatmentAppointment.Anamnesis = request.Anamnesis;
            treatmentAppointment.StatusObservation = request.StatusObservation;
            treatmentAppointment.Therapy = request.Therapy;
            treatmentAppointment.DiagnosedCondition = request.DiagnosedCondition;
            treatmentAppointment.Status = AppointmentStatus.Completed;
            treatmentAppointment.CompletedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(treatmentAppointment);
            return await MapToDtoAsync(treatmentAppointment);
        }

        public async Task<AppointmentDto> CompletePreventiveAppointmentAsync(int id, CompletePreventiveRequest request)
        {
            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found");

            if (appointment is not PreventiveAppointment preventiveAppointment)
                throw new InvalidOperationException("Appointment is not a preventive appointment");

            if (appointment.Status != AppointmentStatus.InProgress)
                throw new InvalidOperationException($"Cannot complete appointment with status: {appointment.Status}");

            preventiveAppointment.PreventiveNotes = request.PreventiveNotes;
            preventiveAppointment.Status = AppointmentStatus.Completed;
            preventiveAppointment.CompletedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(preventiveAppointment);
            return await MapToDtoAsync(preventiveAppointment);
        }

        private async Task<AppointmentDto> MapToDtoAsync(Appointment appointment)
        {
            var dto = new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}",
                PatientEmail = appointment.Patient.Email,
                PatientNoShowCount = appointment.Patient.NoShowCount,
                DoctorId = appointment.DoctorId,
                DoctorName = $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",
                DoctorSpecialty = appointment.Doctor.Specialty,
                ClinicId = appointment.ClinicId,
                ClinicName = appointment.Clinic.Name,
                AppointmentType = appointment.AppointmentType,
                Status = appointment.Status,
                ScheduledDate = appointment.ScheduledDate,
                ScheduledStartTime = appointment.ScheduledStartTime,
                ScheduledEndTime = appointment.ScheduledEndTime,
                BookedAt = appointment.BookedAt,
                BookedByWorkerName = $"{appointment.BookedByWorker.FirstName} {appointment.BookedByWorker.LastName}",
                CheckedInAt = appointment.CheckedInAt,
                CompletedAt = appointment.CompletedAt,
                CancelledAt = appointment.CancelledAt,
                CancellationReason = appointment.CancellationReason
            };

            if (appointment is TreatmentAppointment treatmentAppointment)
            {
                dto.Anamnesis = treatmentAppointment.Anamnesis;
                dto.StatusObservation = treatmentAppointment.StatusObservation;
                dto.Therapy = treatmentAppointment.Therapy;
                dto.DiagnosedCondition = treatmentAppointment.DiagnosedCondition;
            }
            else if (appointment is PreventiveAppointment preventiveAppointment)
            {
                dto.PreventiveNotes = preventiveAppointment.PreventiveNotes;
            }

            return dto;
        }
    }
}