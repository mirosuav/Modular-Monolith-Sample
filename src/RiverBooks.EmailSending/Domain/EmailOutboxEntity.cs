namespace RiverBooks.EmailSending.Domain;

// TODO limit sending tries and log message/mark email as failure in case email cannot be send many times
public class EmailOutboxEntity
{
    public Guid Id { get; set; } = Guid.NewGuid(); // TODO Use DB generated guid or UUIDv7
    public string To { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime? DateTimeUtcProcessed { get; set; } = null!;
}
