using Klinika.DATA;
using Klinika.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Klinika.Services
{
    public class AppointmentNotificationService : IAppointmentNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AppointmentNotificationService> _logger;

        public AppointmentNotificationService(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<AppointmentNotificationService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task NotifyAppointmentCompletedAsync(int appointmentId, CancellationToken cancellationToken = default)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p!.Guardian)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found for completion notification.", appointmentId);
                return;
            }

            var recipient = ResolveRecipient(appointment.Patient);
            if (recipient == null)
            {
                _logger.LogInformation("Skipping completion email for appointment {AppointmentId}: no valid recipient email.", appointmentId);
                return;
            }

            var patientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}";
            var doctorName = $"Dr. {appointment.Doctor.FirstName} {appointment.Doctor.LastName}";
            var scheduledAt = appointment.ScheduledDate.ToDateTime(appointment.ScheduledStartTime);
            var appointmentKind = appointment.AppointmentType == AppointmentType.Preventive ? "preventive" : "treatment";
            var subject = "Appointment completed at Klinika";
            var body = string.Join(Environment.NewLine, new[]
            {
                $"Hello {recipient.Value.Name},",
                string.Empty,
                $"The appointment for {patientName} has been completed.",
                $"Visit type: {appointmentKind}.",
                $"Doctor: {doctorName}.",
                $"Clinic: {appointment.Clinic.Name}.",
                $"Scheduled time: {scheduledAt:dd MMM yyyy HH:mm}.",
                string.Empty,
                "If follow-up care, prescriptions, referrals, or additional instructions are needed, please contact the clinic directly.",
                string.Empty,
                "Klinika",
            });

            await _emailService.SendAsync(recipient.Value.Email, recipient.Value.Name, subject, body, cancellationToken);
        }

        private static (string Email, string Name)? ResolveRecipient(Patient patient)
        {
            var patientEmail = patient.Email?.Trim();
            var guardianEmail = patient.Guardian?.Email?.Trim();
            var guardianName = patient.Guardian == null ? null : $"{patient.Guardian.FirstName} {patient.Guardian.LastName}";
            var patientName = $"{patient.FirstName} {patient.LastName}";
            var isAdult = GetAge(patient.DateOfBirth, DateTime.UtcNow.Date) >= 18;

            if (isAdult && !string.IsNullOrWhiteSpace(patientEmail))
            {
                return (patientEmail, patientName);
            }

            if (!isAdult && !string.IsNullOrWhiteSpace(guardianEmail) && !string.IsNullOrWhiteSpace(guardianName))
            {
                return (guardianEmail, guardianName);
            }

            return null;
        }

        private static int GetAge(DateTime dateOfBirth, DateTime today)
        {
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}