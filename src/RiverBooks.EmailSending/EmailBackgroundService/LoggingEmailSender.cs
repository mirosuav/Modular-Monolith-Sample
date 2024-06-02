using Microsoft.Extensions.Logging;

namespace RiverBooks.EmailSending.EmailBackgroundService;

public class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : ISendEmail
{
    private readonly ILogger<LoggingEmailSender> _logger = logger;

    public Task SendEmailAsync(string to, string from, string subject, string body, CancellationToken cancellationToken)
    {
        // TODO implement concrete email service

        _logger.LogInformation("Attempting to send email to {to} from {from} with subject {subject}...", to, from, subject);

        _logger.LogInformation("Email sent from: {from} to {to}, with subject: {subject} and message: {body}", from, to, subject, body);

        return Task.CompletedTask;
    }
}



