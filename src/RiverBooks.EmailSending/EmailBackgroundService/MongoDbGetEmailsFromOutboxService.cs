using MongoDB.Driver;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.EmailBackgroundService;

internal class MongoDbGetEmailsFromOutboxService(IMongoCollection<EmailOutboxEntity> emailCollection) : IGetEmailsFromOutboxService
{
    private readonly IMongoCollection<EmailOutboxEntity> _emailCollection = emailCollection;

    public async Task<Resultable<EmailOutboxEntity>> GetUnprocessedEmailEntity()
    {
        var filter = Builders<EmailOutboxEntity>.Filter.Eq(entity => entity.DateTimeUtcProcessed, null);
        var unsentEmailEntity = await _emailCollection.Find(filter).FirstOrDefaultAsync();

        if (unsentEmailEntity == null)
            return Error.CreateNotFound("User EmailAddress not found");

        return unsentEmailEntity;
    }
}
