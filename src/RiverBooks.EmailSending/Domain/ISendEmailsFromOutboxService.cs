namespace RiverBooks.EmailSending.Domain;

internal interface ISendEmailsFromOutboxService
{
    Task CheckForAndSendEmails(CancellationToken cancellationToken);
}