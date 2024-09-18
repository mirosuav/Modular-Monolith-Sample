using Microsoft.Extensions.Logging;
using Polly.Registry;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.Application;

internal class DefaultSendEmailsFromOutboxService(
    IGetEmailsFromOutboxService outboxService,
    IMarkEmailProcessed outboxProcessedService,
    IEmailSender emailSender,
    ResiliencePipelineProvider<Type> _resilienceProvider,
    ILogger<DefaultSendEmailsFromOutboxService> logger)
    : ISendEmailsFromOutboxService
{
    public async Task CheckForAndSendEmails(CancellationToken cancellationToken)
    {
        // Retrieve all unprocessed emails and sends them
        var result = await outboxService.GetNextUnprocessedEmailEntity(cancellationToken);

        if (!result.IsSuccess) return;

        var emailEntity = result.Value;

        var pipeline = _resilienceProvider.GetPipeline(typeof(ISendEmailsFromOutboxService));

        try
        {
            await pipeline.ExecuteAsync(async ct =>
                    await emailSender.SendEmailAsync(
                        emailEntity.To,
                        emailEntity.From,
                        emailEntity.Subject,
                        emailEntity.Body, ct),
                cancellationToken);

            await outboxProcessedService.UpdateEmailStatus(emailEntity.Id, EmailProcessingStatus.Success,
                cancellationToken);

            logger.LogInformation("Processed email [{id}] with success.", emailEntity.Id);
        }
        catch (Exception ex)
        {
            await outboxProcessedService.UpdateEmailStatus(emailEntity.Id, EmailProcessingStatus.DeadLetter,
                cancellationToken);
            logger.LogError(ex, "Processing email [{id}] failed and results in DeadLetter.", emailEntity.Id);
        }
    }
}