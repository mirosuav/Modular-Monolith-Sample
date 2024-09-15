using MediatR;
using RiverBooks.OrderProcessing.Application.Interfaces;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Application.ListOrdersForUsers;

internal class ListOrdersForUserQueryHandler(IOrderRepository orderRepository) :
  IRequestHandler<ListOrdersForUserQuery,
  ResultOf<List<OrderSummary>>>
{
    public async Task<ResultOf<List<OrderSummary>>> Handle(ListOrdersForUserQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository
            .ListForUserAsync(request.UserId, cancellationToken);

        var summaries = orders.Select(o => new OrderSummary
        {
            DateCreated = o.DateCreated,
            OrderId = o.Id,
            UserId = o.UserId,
            Total = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice) // need to .Include OrderItems
        })
          .ToList();

        return summaries;
    }
}
