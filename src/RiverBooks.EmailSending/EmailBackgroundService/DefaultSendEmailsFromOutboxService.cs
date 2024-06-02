using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.EmailBackgroundService;

internal class DefaultSendEmailsFromOutboxService(
        IGetEmailsFromOutboxService outboxService,
        IMarkEmailProcessed outboxProcessedService,
        ISendEmail emailSender,
        ILogger<DefaultSendEmailsFromOutboxService> logger) 
    : ISendEmailsFromOutboxService
{
    private readonly IGetEmailsFromOutboxService _outboxService = outboxService;
    private readonly IMarkEmailProcessed outboxProcessedService = outboxProcessedService;
    private readonly ISendEmail _emailSender = emailSender;
    private readonly ILogger<DefaultSendEmailsFromOutboxService> _logger = logger;

    public async Task CheckForAndSendEmails(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _outboxService.GetUnprocessedEmailEntity(cancellationToken);

            if (!result.IsSuccess) return;

            var emailEntity = result.Value;

            await _emailSender.SendEmailAsync(emailEntity.To,
              emailEntity.From,
              emailEntity.Subject,
              emailEntity.Body,
              cancellationToken);

            await outboxProcessedService.MarkEmailSend(emailEntity.Id, cancellationToken);

            _logger.LogInformation("Processed email [{id}].", emailEntity.Id);
        }
        finally
        {
            _logger.LogInformation("Sleeping...");
        }

    }
}
