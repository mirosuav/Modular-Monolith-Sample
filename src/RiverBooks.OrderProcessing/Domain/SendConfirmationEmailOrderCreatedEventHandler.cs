using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Domain;

internal class SendConfirmationEmailOrderCreatedEventHandler(
    IMediator mediatR,
    ILogger<SendConfirmationEmailOrderCreatedEventHandler> logger) :
    INotificationHandler<OrderCreatedDomainEvent>
{
    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken ct)
    {
        var userByIdQuery = new UserDetailsByIdQuery(notification.Order.UserId);

        var result = await mediatR.Send(userByIdQuery, ct);

        if (!result.IsSuccess)
        {
            logger.LogError("Could not retrieve user details from User module.");
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

        await mediatR.Publish(command, ct);
    }
}

