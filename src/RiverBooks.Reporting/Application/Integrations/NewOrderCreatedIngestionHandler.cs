using MediatR;
using Microsoft.Extensions.Logging;
using OrderProcessing.Contracts;
using RiverBooks.Books.Contracts;
using RiverBooks.Reporting.Domain;
using RiverBooks.Reporting.Infrastructure;

namespace RiverBooks.Reporting.Application.Integrations;

internal class NewOrderCreatedIngestionHandler(
    ILogger<NewOrderCreatedIngestionHandler> logger,
    OrderIngestionService orderIngestionService,
    IMediator mediator) :
    INotificationHandler<OrderCreatedIntegrationEvent>
{
    public async Task Handle(OrderCreatedIntegrationEvent notification,
      CancellationToken ct)
    {
        logger.LogInformation("Handling order created event to populate reporting database...");

        var orderItems = notification.OrderDetails.OrderItems;
        int year = notification.OrderDetails.DateCreated.Year;
        int month = notification.OrderDetails.DateCreated.Month;

        foreach (var item in orderItems)
        {
            // look up book details to get author and title
            // TODO: Implement Materialized View or other cache
            var bookDetailsQuery = new BookDetailsQuery(item.BookId);
            var result = await mediator.Send(bookDetailsQuery, ct);

            if (!result.IsSuccess)
            {
                logger.LogWarning("Issue loading book details for {id}", item.BookId);
                continue;
            }

            string author = result.Value.Author;
            string title = result.Value.Title;

            var sale = new BookSale
            {
                Author = author,
                BookId = item.BookId,
                Month = month,
                Title = title,
                Year = year,
                TotalSales = item.Quantity * item.UnitPrice,
                UnitsSold = item.Quantity
            };

            await orderIngestionService.AddOrUpdateMonthlyBookSalesAsync(sale);
        }
    }
}
