using MediatR;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.Application;

internal class QueueEmailInOutboxSendEmailCommandHandler(IQueueEmailsInOutboxService outboxService)
    : INotificationHandler<SendEmailCommand>
{
    public async Task Handle(SendEmailCommand request, CancellationToken ct)
    {
        var newEntity = new EmailOutboxEntity
        {
            Body = request.Body,
            Subject = request.Subject,
            To = request.To,
            From = request.From
        };

        await outboxService.QueueEmailForSending(newEntity, ct);
    }
}
