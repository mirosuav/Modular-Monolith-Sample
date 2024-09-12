using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.Application;

internal class EmailSendingBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<EmailSendingBackgroundService> logger) : BackgroundService
{
    private const int CheckEmailsInterval = 1_000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{serviceName} started.", nameof(EmailSendingBackgroundService));

        using var scope = scopeFactory.CreateScope();

        var sendEmailsFromOutboxService = scope.ServiceProvider.GetRequiredService<ISendEmailsFromOutboxService>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await sendEmailsFromOutboxService.CheckForAndSendEmails(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError("Error processing email outbox: {message}", ex.Message);
            }
            finally
            {
                logger.LogTrace("Sleeping {checkEmailsInterval}ms...", CheckEmailsInterval);
                await Task.Delay(CheckEmailsInterval, stoppingToken);
            }
        }

        logger.LogInformation("{serviceName} stopping.", nameof(EmailSendingBackgroundService));
    }
}
