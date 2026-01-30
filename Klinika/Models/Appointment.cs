namespace Klinika.Models
{
    public abstract class Appointment
    {
        // Core Identity
        public int Id { get; set; }
        public int AppointmentSlotId { get; set; }
        public int PatientId { get; set; }
        public int ClinicId { get; set; }
        public int DoctorId { get; set; }
        
        // Appointment Details
        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateOnly ScheduledDate { get; set; }
        public TimeOnly ScheduledStartTime { get; set; }
        public TimeOnly ScheduledEndTime { get; set; }
        
        // Booking Info
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
        public int BookedByWorkerId { get; set; }
        
        // Lifecycle Timestamps
        public DateTime? CheckedInAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        
        // Navigation Properties
        public AppointmentSlot AppointmentSlot { get; set; }
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Clinic Clinic { get; set; }
        public Worker BookedByWorker { get; set; }
    }
}