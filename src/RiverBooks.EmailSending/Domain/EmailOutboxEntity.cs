using RiverBooks.SharedKernel;

namespace RiverBooks.EmailSending.Domain;

// TODO limit sending tries and log message/mark email as failure in case email cannot be sent many times
public class EmailOutboxEntity
{
    public Guid Id { get; set; } = SequentialGuid.NewGuid();
    public string To { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime? DateTimeUtcProcessed { get; set; } = null!;
}
