namespace RiverBooks.EmailSending.Domain;

internal interface IQueueEmailsInOutboxService
{
    Task QueueEmailForSending(EmailOutboxEntity entity, CancellationToken cancellationToken);
}
