using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.ListOrdersForUser;

internal class ListOrdersForUserQueryHandler(IOrderRepository orderRepository) :
  IRequestHandler<ListOrdersForUserQuery,
  Resultable<List<OrderSummary>>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<Resultable<List<OrderSummary>>> Handle(ListOrdersForUserQuery request, CancellationToken cancellationToken)
    {
        // look up UserId for EmailAddress

        // TODO: Filter by User
        var orders = await _orderRepository.ListAsync(cancellationToken);

        var summaries = orders.Select(o => new OrderSummary
        {
            DateCreated = o.DateCreated,
            OrderId = o.Id,
            UserId = o.UserId,
            Total = o.OrderItems.Sum(oi => oi.UnitPrice) // need to .Include OrderItems
        })
          .ToList();

        return summaries;
    }
}
