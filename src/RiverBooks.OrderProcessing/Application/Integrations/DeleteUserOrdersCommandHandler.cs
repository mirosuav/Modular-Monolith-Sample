using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Application.Interfaces;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Application.Integrations;

internal class DeleteUserOrdersCommandHandler(
    IOrderRepository orderRepository,
    ILogger<DeleteUserOrdersCommandHandler> logger)
    : IRequestHandler<DeleteUserOrdersCommand, Resultable>
{
    public async Task<Resultable> Handle(DeleteUserOrdersCommand request, CancellationToken cancellationToken)
    {
        var deletedOrdersCount = await orderRepository.DeleteForUserAsync(request.UserId, cancellationToken);

        if (deletedOrdersCount > 0)
            logger.LogInformation("Orders for user {userId} removed", request.UserId);

        return Resultable.Success();
    }
}
