namespace RiverBooks.EmailSending.Integrations;

internal interface IQueueEmailsInOutboxService
{
    Task QueueEmailForSending(EmailOutboxEntity entity);
}
