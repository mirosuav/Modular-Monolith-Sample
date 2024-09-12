namespace RiverBooks.EmailSending.Domain;

internal interface IMarkEmailProcessed
{
    Task UpdateEmailStatus(Guid emailId, EmailProcessingStatus status, CancellationToken cancellationToken);
}
