using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.EmailBackgroundService;

internal class EmailSendingBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<EmailSendingBackgroundService> logger) : BackgroundService
{
    private const int checkEmailsInterval = 5_000;
    private readonly IServiceScopeFactory scopeFactory = scopeFactory;
    private readonly ILogger<EmailSendingBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{serviceName} starting...", nameof(EmailSendingBackgroundService));

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
                _logger.LogError("Error processing outbox: {message}", ex.Message);
            }
            finally
            {
                _logger.LogDebug("Sleeping {checkEmailsInterval}ms...", checkEmailsInterval);
                await Task.Delay(checkEmailsInterval, stoppingToken);
            }
        }

        _logger.LogInformation("{serviceName} stopping.", nameof(EmailSendingBackgroundService));
    }
}
