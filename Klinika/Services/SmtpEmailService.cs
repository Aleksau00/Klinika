using Klinika.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Klinika.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendAsync(
            string toEmail,
            string toName,
            string subject,
            string plainTextBody,
            IReadOnlyCollection<EmailAttachment>? attachments = null,
            CancellationToken cancellationToken = default)
        {
            if (!_settings.IsConfigured())
            {
                _logger.LogWarning("Email settings are incomplete; skipping outgoing email to {Recipient}.", toEmail);
                return;
            }

            var attachmentStreams = new List<MemoryStream>();

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = plainTextBody,
                IsBodyHtml = false,
            };
            message.To.Add(new MailAddress(toEmail, toName));

            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    var stream = new MemoryStream(file.Content);
                    attachmentStreams.Add(stream);

                    var mailAttachment = new Attachment(stream, file.FileName, file.ContentType ?? MediaTypeNames.Application.Octet);
                    message.Attachments.Add(mailAttachment);
                }
            }

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.UseSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            };

            try
            {
                await client.SendMailAsync(message, cancellationToken);
            }
            finally
            {
                foreach (var stream in attachmentStreams)
                {
                    stream.Dispose();
                }
            }
        }
    }
}