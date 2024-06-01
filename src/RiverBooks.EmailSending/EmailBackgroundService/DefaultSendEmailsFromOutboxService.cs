using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace RiverBooks.EmailSending.EmailBackgroundService;

internal class DefaultSendEmailsFromOutboxService(IGetEmailsFromOutboxService outboxService,
  ISendEmail emailSender,
  IMongoCollection<EmailOutboxEntity> emailCollection,
  ILogger<DefaultSendEmailsFromOutboxService> logger) : ISendEmailsFromOutboxService
{
    private readonly IGetEmailsFromOutboxService _outboxService = outboxService;
    private readonly ISendEmail _emailSender = emailSender;
    private readonly IMongoCollection<EmailOutboxEntity> _emailCollection = emailCollection;
    private readonly ILogger<DefaultSendEmailsFromOutboxService> _logger = logger;

    public async Task CheckForAndSendEmails()
    {
        try
        {
            var result = await _outboxService.GetUnprocessedEmailEntity();

            if (!result.IsSuccess) return;

            var emailEntity = result.Value;

            await _emailSender.SendEmailAsync(emailEntity.To,
              emailEntity.From,
              emailEntity.Subject,
              emailEntity.Body);

            var updateFilter = Builders<EmailOutboxEntity>
              .Filter.Eq(x => x.Id, emailEntity.Id);
            var update = Builders<EmailOutboxEntity>
              .Update.Set("DateTimeUtcProcessed", DateTime.UtcNow);
            var updateResult = await _emailCollection
              .UpdateOneAsync(updateFilter, update);

            _logger.LogInformation("Processed {result} email records.",
              updateResult.ModifiedCount);
        }
        finally
        {
            _logger.LogInformation("Sleeping...");
        }

    }
}
