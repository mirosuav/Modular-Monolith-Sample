namespace RiverBooks.EmailSending.Domain;

internal interface IMarkEmailProcessed
{
    Task MarkEmailSend(Guid emailId, CancellationToken cancellationToken);
}
