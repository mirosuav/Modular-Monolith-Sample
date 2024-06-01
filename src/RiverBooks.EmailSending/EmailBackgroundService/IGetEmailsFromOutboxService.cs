using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.EmailBackgroundService;

internal interface IGetEmailsFromOutboxService
{
    Task<Resultable<EmailOutboxEntity>> GetUnprocessedEmailEntity();
}
