using MediatR;
using Microsoft.Extensions.Logging;
using OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiverBooks.OrderProcessing.Integrations;

internal class DeleteUserOrdersCommandHandler(
    IOrderRepository orderRepository,
    ILogger<DeleteUserOrdersCommandHandler> logger)
    : IRequestHandler<DeleteUserOrdersCommand, Resultable>
{
    private readonly IOrderRepository orderRepository = orderRepository;
    private readonly ILogger<DeleteUserOrdersCommandHandler> logger = logger;

    public async Task<Resultable> Handle(DeleteUserOrdersCommand request, CancellationToken cancellationToken)
    {
        var userOrders = await orderRepository.ListForUserAsync(request.UserId, cancellationToken);

        if (userOrders.Count == 0)
            return Resultable.Success();

        orderRepository.Remove([..userOrders]);

        await orderRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Orders for user {userId} removed", request.UserId);

        return Resultable.Success();
    }
}
