using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.EmailBackgroundService;

internal class EmailSendingBackgroundService(
    ILogger<EmailSendingBackgroundService> logger,
    ISendEmailsFromOutboxService sendEmailsFromOutboxService) : BackgroundService
{
    private const int checkEmailsInterval = 5_000;
    private readonly ILogger<EmailSendingBackgroundService> _logger = logger;
    private readonly ISendEmailsFromOutboxService _sendEmailsFromOutboxService = sendEmailsFromOutboxService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{serviceName} starting...", nameof(EmailSendingBackgroundService));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _sendEmailsFromOutboxService.CheckForAndSendEmails(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing outbox: {message}", ex.Message);
            }
            finally
            {
                await Task.Delay(checkEmailsInterval, stoppingToken);
            }
        }

        _logger.LogInformation("{serviceName} stopping.", nameof(EmailSendingBackgroundService));
    }
}
