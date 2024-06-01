using MediatR;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.Integrations;

internal class QueueEmailInOutboxSendEmailCommandHandler(IQueueEmailsInOutboxService outboxService) : IRequestHandler<SendEmailCommand, Resultable<Guid>>
{
    private readonly IQueueEmailsInOutboxService _outboxService = outboxService;

    public async Task<Resultable<Guid>> Handle(SendEmailCommand request, CancellationToken ct)
    {
        var newEntity = new EmailOutboxEntity
        {
            Body = request.Body,
            Subject = request.Subject,
            To = request.To,
            From = request.From
        };

        await _outboxService.QueueEmailForSending(newEntity);

        return newEntity.Id;
    }
}
