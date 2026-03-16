namespace Klinika.Services
{
    public interface IAppointmentNotificationService
    {
        Task NotifyAppointmentCompletedAsync(int appointmentId, CancellationToken cancellationToken = default);
    }
}