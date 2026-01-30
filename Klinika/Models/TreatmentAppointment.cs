namespace Klinika.Models
{
    public class TreatmentAppointment : Appointment
    {
        public TreatmentAppointment()
        {
            AppointmentType = AppointmentType.Treatment;
        }
        public string? Anamnesis { get; set; }        // Patient's complaint/symptoms
        public string? StatusObservation { get; set; } // What doctor observed
        public string? Therapy { get; set; }          // Treatment plan/prescription
        public string? DiagnosedCondition { get; set; } // Doctor's diagnosis
        
        
    }
}