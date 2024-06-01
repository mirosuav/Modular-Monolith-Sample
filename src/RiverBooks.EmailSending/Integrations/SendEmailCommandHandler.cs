using RiverBooks.EmailSending.Contracts;
using RiverBooks.EmailSending.EmailBackgroundService;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.Integrations;
internal class SendEmailCommandHandler(ISendEmail emailSender) //:  IRequestHandler<SendEmailCommand, ResultOr<Guid>>
{
    private readonly ISendEmail _emailSender = emailSender;

    public async Task<Resultable<Guid>> HandleAsync(SendEmailCommand request, CancellationToken ct)
    {
        await _emailSender.SendEmailAsync(request.To,
          request.From,
          request.Subject,
          request.Body);

        return Guid.Empty;
    }
}
