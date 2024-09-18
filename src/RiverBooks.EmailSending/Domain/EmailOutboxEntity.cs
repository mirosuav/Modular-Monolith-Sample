using System.ComponentModel.DataAnnotations;
using RiverBooks.SharedKernel;

namespace RiverBooks.EmailSending.Domain;

// TODO limit sending tries and log message/mark email as failure in case email cannot be sent many times
public class EmailOutboxEntity
{
    public Guid Id { get; set; } = SequentialGuid.NewGuid();

    public DateTime IssuedAtUtc { get; init; }

    [StringLength(100)] public string To { get; set; } = string.Empty;

    [StringLength(100)] public string From { get; set; } = string.Empty;

    [StringLength(500)] public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;
    public EmailProcessingStatus Status { get; set; }
    public DateTime? ProcessedAtUtc { get; set; } = null!;
}

public enum EmailProcessingStatus
{
    Pending = 0,
    Success = 1,
    DeadLetter = 2
}