using MediatR;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.EmailSending.EmailBackgroundService;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.Integrations;

internal class SendEmailCommandHandler(ISendEmail emailSender)
    : IRequestHandler<SendEmailCommand, Resultable<Guid>>
{
    private readonly ISendEmail _emailSender = emailSender;

    public async Task<Resultable<Guid>> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        await _emailSender.SendEmailAsync(
            request.To,
            request.From,
            request.Subject,
            request.Body,
            cancellationToken);

        return Guid.Empty;
    }
}
