using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.Application;

internal class EmailSendingBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<EmailSendingBackgroundService> logger) : BackgroundService
{
    private const int checkEmailsInterval = 5_000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{serviceName} starting...", nameof(EmailSendingBackgroundService));

        using var scope = scopeFactory.CreateScope();

        var _sendEmailsFromOutboxService = scope.ServiceProvider.GetRequiredService<ISendEmailsFromOutboxService>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _sendEmailsFromOutboxService.CheckForAndSendEmails(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError("Error processing outbox: {message}", ex.Message);
            }
            finally
            {
                logger.LogDebug("Sleeping {checkEmailsInterval}ms...", checkEmailsInterval);
                await Task.Delay(checkEmailsInterval, stoppingToken);
            }
        }

        logger.LogInformation("{serviceName} stopping.", nameof(EmailSendingBackgroundService));
    }
}
