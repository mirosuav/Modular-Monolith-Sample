using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Contracts;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.Reporting.Domain;

namespace RiverBooks.Reporting.Application.Integrations;

internal class OrderCreatedIntegrationEventHandler(
    ILogger<OrderCreatedIntegrationEventHandler> logger,
    ISalesReportRepository bookSaleRepository,
    IMediator mediator) :
    INotificationHandler<OrderCreatedIntegrationEvent>
{
    public async Task Handle(
        OrderCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling order created event to populate reporting database...");
        
        foreach (var item in notification.OrderDetails.OrderItems)
        {
            // look up book details from Books module to get author and title
            var bookDetailsQuery = new BookDetailsQuery(item.BookId);
            var result = await mediator.Send(bookDetailsQuery, cancellationToken);

            if (!result.IsSuccess)
            {
                logger.LogWarning("Issue loading book details for {id}", item.BookId);
                continue;
            }
            
            var sale = new BookSale
            {
                OrderId = notification.OrderDetails.OrderId,
                CustomerId = notification.OrderDetails.UserId,
                Author = result.Value.Author,
                BookId = item.BookId,
                Title = result.Value.Title,
                TotalSales = item.Quantity * item.UnitPrice,
                UnitsSold = item.Quantity,
                SoldAtUtc = notification.OrderDetails.DateCreated.DateTime
            };

            bookSaleRepository.AddBookSale(sale);
        }

        await bookSaleRepository.SaveChangesAsync(cancellationToken);
    }
}
