﻿using Microsoft.Extensions.Logging;

namespace RiverBooks.EmailSending.Application;

/// <summary>
/// Some testing email sender - sends emails to log
/// </summary>
public class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendEmailAsync(string to, string from, string subject, string body, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to send email to {to} from {from} with subject {subject}...", to, from, subject);

        logger.LogInformation("Email sent from: {from} to {to}, with subject: {subject} and message: {body}", from, to, subject, body);

        return Task.CompletedTask;
    }
}


