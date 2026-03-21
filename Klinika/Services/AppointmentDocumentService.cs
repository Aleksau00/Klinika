using Klinika.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Klinika.Services
{
    public class AppointmentDocumentService : IAppointmentDocumentService
    {
        public AppointmentDocumentService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public EmailAttachment CreateTreatmentReceipt(TreatmentAppointment appointment, string clinicName, string doctorDisplayName, string patientDisplayName)
        {
            var fileName = $"treatment-receipt-{appointment.Id}.pdf";

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(32);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("Klinika").FontSize(22).Bold();
                        column.Item().Text("Treatment Appointment Receipt").FontSize(14).SemiBold().FontColor(Colors.Grey.Darken2);
                        column.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingTop(16).Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text("Receipt Information").SemiBold().FontSize(13);
                        column.Item().Text($"Receipt ID: TR-{appointment.Id:D6}");
                        column.Item().Text($"Appointment ID: {appointment.Id}");
                        column.Item().Text($"Generated at: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC");

                        column.Item().PaddingTop(6).Text("Visit Details").SemiBold().FontSize(13);
                        column.Item().Text($"Clinic: {clinicName}");
                        column.Item().Text($"Patient: {patientDisplayName}");
                        column.Item().Text($"Doctor: {doctorDisplayName}");
                        column.Item().Text($"Scheduled: {appointment.ScheduledDate:yyyy-MM-dd} {appointment.ScheduledStartTime:HH\\:mm} - {appointment.ScheduledEndTime:HH\\:mm}");
                        column.Item().Text($"Completed: {(appointment.CompletedAt.HasValue ? appointment.CompletedAt.Value.ToString("yyyy-MM-dd HH:mm") + " UTC" : "N/A")}");

                        column.Item().PaddingTop(6).Text("Clinical Summary").SemiBold().FontSize(13);
                        column.Item().Text($"Diagnosed condition: {appointment.DiagnosedCondition ?? "N/A"}");
                        column.Item().Text($"Anamnesis: {appointment.Anamnesis ?? "N/A"}");
                        column.Item().Text($"Status observation: {appointment.StatusObservation ?? "N/A"}");
                        column.Item().Text($"Therapy: {appointment.Therapy ?? "N/A"}");

                        column.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        column.Item().Text("Doctor Signature").SemiBold();
                        column.Item().Text($"Signed by: {doctorDisplayName}");
                        column.Item().Text("Signature type: System-generated professional signature").FontColor(Colors.Grey.Darken1);
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Klinika Receipt ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf();

            return new EmailAttachment(fileName, "application/pdf", pdf);
        }
    }
}
