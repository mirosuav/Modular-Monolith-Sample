using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using Polly.Retry;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.EmailBackgroundService;

internal class DefaultSendEmailsFromOutboxService(
        IGetEmailsFromOutboxService outboxService,
        IMarkEmailProcessed outboxProcessedService,
        IEmailSender emailSender,
        ResiliencePipelineProvider<Type> _resilienceProvider,
        ILogger<DefaultSendEmailsFromOutboxService> logger)
    : ISendEmailsFromOutboxService
{
    private readonly IGetEmailsFromOutboxService _outboxService = outboxService;
    private readonly IMarkEmailProcessed outboxProcessedService = outboxProcessedService;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ResiliencePipelineProvider<Type> resilienceProvider = _resilienceProvider;
    private readonly ILogger<DefaultSendEmailsFromOutboxService> _logger = logger;

    public async Task CheckForAndSendEmails(CancellationToken cancellationToken)
    {
        var result = await _outboxService.GetUnprocessedEmailEntity(cancellationToken);

        if (!result.IsSuccess) return;

        var emailEntity = result.Value;

        var pipeline = resilienceProvider.GetPipeline(typeof(IEmailSender));

        await pipeline.ExecuteAsync(async (ct) =>
            await _emailSender.SendEmailAsync(emailEntity.To,
              emailEntity.From,
              emailEntity.Subject,
              emailEntity.Body,
              ct)
            , cancellationToken);

        await outboxProcessedService.MarkEmailSend(emailEntity.Id, cancellationToken);

        _logger.LogInformation("Processed email [{id}].", emailEntity.Id);
    }
}
