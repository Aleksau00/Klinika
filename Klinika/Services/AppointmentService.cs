using Klinika.DATA;
using Klinika.Models;
using Klinika.Models.DTOs;
using Klinika.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<AppointmentDto>> GetDoctorScheduleAsync(int doctorId, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId, fromDate, toDate);
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

        public async Task<AppointmentDto> CheckInPatientAsync(int appointmentId, int? actorDoctorId = null)
        {
            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found");

            if (actorDoctorId.HasValue && appointment.DoctorId != actorDoctorId.Value)
                throw new InvalidOperationException("Doctor can only start appointments assigned to them.");

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

            if (appointment.Status != AppointmentStatus.InProgress && appointment.Status != AppointmentStatus.Scheduled)
                throw new InvalidOperationException($"Cannot complete appointment with status: {appointment.Status}");

            if (appointment.Status == AppointmentStatus.Scheduled)
            {
                appointment.Status = AppointmentStatus.InProgress;
                appointment.CheckedInAt = DateTime.UtcNow;
            }

            treatmentAppointment.Anamnesis = request.Anamnesis;
            treatmentAppointment.StatusObservation = request.StatusObservation;
            treatmentAppointment.Therapy = request.Therapy;
            treatmentAppointment.DiagnosedCondition = request.DiagnosedCondition;
            treatmentAppointment.Status = AppointmentStatus.Completed;
            treatmentAppointment.CompletedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(treatmentAppointment);
            return await MapToDtoAsync(treatmentAppointment);
        }

        public async Task<AppointmentDto> CompletePreventiveAppointmentAsync(int id, CompletePreventiveRequest request, int doctorId)
        {
            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found");

            if (appointment is not PreventiveAppointment preventiveAppointment)
                throw new InvalidOperationException("Appointment is not a preventive appointment");

            if (preventiveAppointment.DoctorId != doctorId)
                throw new InvalidOperationException("Doctor can only complete preventive appointments assigned to them.");

            if (appointment.Status != AppointmentStatus.InProgress && appointment.Status != AppointmentStatus.Scheduled)
                throw new InvalidOperationException($"Cannot complete appointment with status: {appointment.Status}");

            if (appointment.Status == AppointmentStatus.Scheduled)
            {
                appointment.Status = AppointmentStatus.InProgress;
                appointment.CheckedInAt = DateTime.UtcNow;
            }

            var hasPreventiveContent = !string.IsNullOrWhiteSpace(request.PreventiveNotes)
                || !string.IsNullOrWhiteSpace(request.ChildDevelopmentNotes);
            var wantsVaccination = request.IsVaccination == true || request.VaccinationId.HasValue;

            if (!hasPreventiveContent && !wantsVaccination)
                throw new InvalidOperationException("Provide preventive notes, child development notes, or vaccination details.");

            preventiveAppointment.PreventiveNotes = request.PreventiveNotes;
            preventiveAppointment.ChildDevelopmentNotes = request.ChildDevelopmentNotes;

            if (wantsVaccination)
            {
                if (!request.VaccinationId.HasValue)
                    throw new InvalidOperationException("VaccinationId is required when recording vaccination in preventive completion.");

                var vaccination = await _context.Vaccinations.FindAsync(request.VaccinationId.Value);
                if (vaccination == null)
                    throw new KeyNotFoundException($"Vaccination with ID {request.VaccinationId.Value} not found");

                preventiveAppointment.IsVaccination = true;
                preventiveAppointment.VaccinationId = request.VaccinationId.Value;

                var existingRecord = await _context.VaccinationRecords
                    .FirstOrDefaultAsync(v => v.PreventiveAppointmentId == preventiveAppointment.Id);

                if (existingRecord == null)
                {
                    _context.VaccinationRecords.Add(new VaccinationRecord
                    {
                        PatientId = preventiveAppointment.PatientId,
                        VaccinationId = request.VaccinationId.Value,
                        AdministeredDate = DateTime.UtcNow,
                        Notes = request.VaccinationNotes,
                        AdministeredByDoctorId = doctorId,
                        PreventiveAppointmentId = preventiveAppointment.Id
                    });
                }
                else
                {
                    existingRecord.VaccinationId = request.VaccinationId.Value;
                    existingRecord.Notes = request.VaccinationNotes;
                    existingRecord.AdministeredByDoctorId = doctorId;
                    existingRecord.AdministeredDate = DateTime.UtcNow;
                }
            }

            preventiveAppointment.Status = AppointmentStatus.Completed;
            preventiveAppointment.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
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
                dto.ChildDevelopmentNotes = preventiveAppointment.ChildDevelopmentNotes;
                dto.IsVaccination = preventiveAppointment.IsVaccination;
                dto.VaccinationId = preventiveAppointment.VaccinationId;
                dto.VaccinationNotes = preventiveAppointment.VaccinationRecord?.Notes;
            }

            return dto;
        }
    }
}