namespace Klinika.Models.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }

        // Patient Info
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientEmail { get; set; }
        public int PatientNoShowCount { get; set; }

        // Doctor Info
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialty { get; set; }

        // Clinic Info
        public int ClinicId { get; set; }
        public string ClinicName { get; set; }

        // Appointment Details
        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateOnly ScheduledDate { get; set; }
        public TimeOnly ScheduledStartTime { get; set; }
        public TimeOnly ScheduledEndTime { get; set; }

        // Booking Info
        public DateTime BookedAt { get; set; }
        public string BookedByWorkerName { get; set; }

        // Treatment Fields (null if Preventive)
        public string? Anamnesis { get; set; }
        public string? StatusObservation { get; set; }
        public string? Therapy { get; set; }
        public string? DiagnosedCondition { get; set; }

        // Preventive Fields (null if Treatment)
        public string? PreventiveNotes { get; set; }

        // Timestamps
        public DateTime? CheckedInAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
    }

    public class CreateAppointmentRequest
    {
        public int AppointmentSlotId { get; set; }
        public int PatientId { get; set; }
        public AppointmentType AppointmentType { get; set; }
    }

    public class CompleteTreatmentRequest
    {
        public string Anamnesis { get; set; }
        public string StatusObservation { get; set; }
        public string Therapy { get; set; }
        public string DiagnosedCondition { get; set; }
    }

    public class CompletePreventiveRequest
    {
        public string PreventiveNotes { get; set; }
    }

    public class CancelAppointmentRequest
    {
        public string Reason { get; set; }
    }
}