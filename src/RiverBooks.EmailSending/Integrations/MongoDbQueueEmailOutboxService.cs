using MongoDB.Driver;

namespace RiverBooks.EmailSending.Integrations;

internal class MongoDbQueueEmailOutboxService(IMongoCollection<EmailOutboxEntity> emailCollection) : IQueueEmailsInOutboxService
{
    private readonly IMongoCollection<EmailOutboxEntity> _emailCollection = emailCollection;

    public async Task QueueEmailForSending(EmailOutboxEntity entity)
    {
        await _emailCollection.InsertOneAsync(entity);
    }
}
