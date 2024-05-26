using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.Contracts;

public class SendEmailCommand : IRequest<ResultOr<Guid>>
{
    public string To { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
