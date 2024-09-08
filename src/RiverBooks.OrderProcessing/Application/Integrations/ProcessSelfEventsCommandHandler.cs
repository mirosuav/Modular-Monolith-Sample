using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Infrastructure.Data;
using RiverBooks.SharedKernel.Events;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Application.Integrations;
internal class ProcessSelfEventsCommandHandler(
    OrderProcessingDbContext dbContext,
    IMediator mediator,
    ILogger<ProcessSelfEventsCommandHandler> logger,
    TimeProvider timeProvider) :
    ProcessSelfEventsCommandHandlerBase<OrderProcessingDbContext>(dbContext, mediator, logger, timeProvider)
{
    // Separate semaphore per concrete type
    private static readonly SemaphoreSlim _accessLocker = new(1, 1);
    protected override SemaphoreSlim AccessLocker => _accessLocker;

    protected override async Task PublishOutboxEvent(object domainEvent, CancellationToken cancellationToken)
    {
        // Enrich, transform and process OrderCreatedEvent
        switch (domainEvent)
        {
            case OrderCreated_PrepareReportEvent reportingEvent:
                {
                    var order = await FetchOrder(cancellationToken, reportingEvent.OrderId);

                    await Mediator.Publish(new OrderCreatedIntegrationEvent(order.ToOrderDto()), cancellationToken);

                    break;
                }

            case OrderCreated_SendEmailEvent emailNotificationEvent:
                {
                    var order = await FetchOrder(cancellationToken, emailNotificationEvent.OrderId);

                    var userByIdQuery = new UserDetailsByIdQuery(order.UserId);

                    var result = await Mediator.Send(userByIdQuery, cancellationToken);

                    if (!result.IsSuccess)
                    {
                        throw new ApplicationException("Could not retrieve user details from User module.");
                    }

                    string userEmail = result.Value.EmailAddress;

                    var command = new SendEmailCommand()
                    {
                        To = userEmail,
                        From = "noreply@test.com",
                        Subject = "Your RiverBooks Purchase",
                        Body = $"You bought {order.OrderItems.Count} items."
                    };

                    await Mediator.Publish(command, cancellationToken);
                    break;
                }
            default:
                await base.PublishOutboxEvent(domainEvent, cancellationToken);
                break;
        }
    }

    private async Task<Order> FetchOrder(CancellationToken cancellationToken, Guid orderId)
    {
        var order = await DbContext.Orders
            .Include(x => x.OrderItems)
            .SingleOrDefaultAsync(x => x.Id == orderId, cancellationToken);

        if (order is null)
            throw new ApplicationException($"Could not find Order [{orderId}].");

        return order;
    }
}
