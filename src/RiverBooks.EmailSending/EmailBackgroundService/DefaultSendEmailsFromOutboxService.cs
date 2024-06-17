using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
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
        var result = await _outboxService.GetUnprocessedEmailEntity(cancellationToken);

        if (!result.IsSuccess) return;

        var emailEntity = result.Value;

        // TODO configure it in bootsrtap
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                MaxRetryAttempts = 3
            })
            .AddTimeout(TimeSpan.FromSeconds(30))
            .Build();

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
