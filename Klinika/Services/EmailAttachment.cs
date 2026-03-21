namespace Klinika.Services
{
    public sealed class EmailAttachment
    {
        public string FileName { get; }
        public string ContentType { get; }
        public byte[] Content { get; }

        public EmailAttachment(string fileName, string contentType, byte[] content)
        {
            FileName = fileName;
            ContentType = contentType;
            Content = content;
        }
    }
}
