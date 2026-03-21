using Klinika.Models;

namespace Klinika.Services
{
    public interface IAppointmentDocumentService
    {
        EmailAttachment CreateTreatmentReceipt(TreatmentAppointment appointment, string clinicName, string doctorDisplayName, string patientDisplayName);
    }
}
