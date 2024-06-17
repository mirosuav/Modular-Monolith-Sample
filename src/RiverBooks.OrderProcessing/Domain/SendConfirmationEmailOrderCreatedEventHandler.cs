using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Domain;

internal class SendConfirmationEmailOrderCreatedEventHandler(
    ISender sender, 
    ILogger<SendConfirmationEmailOrderCreatedEventHandler> logger) : 
    INotificationHandler<OrderCreatedEvent>
{
    private readonly ISender _sender = sender;
    private readonly ILogger<SendConfirmationEmailOrderCreatedEventHandler> _logger = logger;

    public async Task Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        var userByIdQuery = new UserDetailsByIdQuery(notification.Order.UserId);

        var result = await _sender.Send(userByIdQuery, ct);

        if (!result.IsSuccess)
        {
            _logger.LogError("Could not retreive user details from User module.");
            return;
        }
        string userEmail = result.Value.EmailAddress;

        var command = new SendEmailCommand()
        {
            To = userEmail,
            From = "noreply@test.com",
            Subject = "Your RiverBooks Purchase",
            Body = $"You bought {notification.Order.OrderItems.Count} items."
        };

        var emailId = await _sender.Send(command, ct);

        _logger.LogInformation("Email with id {EmailId} was send to processing outbox.", emailId);

    }
}

