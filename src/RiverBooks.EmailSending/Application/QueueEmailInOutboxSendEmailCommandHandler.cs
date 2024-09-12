using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.EmailSending.Domain;
using RiverBooks.SharedKernel.Extensions;

namespace RiverBooks.EmailSending.Application;

internal class QueueEmailInOutboxSendEmailCommandHandler(IQueueEmailsInOutboxService outboxService, TimeProvider timeProvider)
    : INotificationHandler<SendEmailCommand>
{
    public async Task Handle(SendEmailCommand request, CancellationToken ct)
    {
        var newEntity = new EmailOutboxEntity
        {
            Status = EmailProcessingStatus.Pending,
            Body = request.Body,
            Subject = request.Subject,
            To = request.To,
            From = request.From,
            IssuedAtUtc = timeProvider.GetUtcDateTime(),
        };

        await outboxService.QueueEmailForSending(newEntity, ct);
    }
}
