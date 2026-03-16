namespace Klinika.Services
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string toName, string subject, string plainTextBody, CancellationToken cancellationToken = default);
    }
}