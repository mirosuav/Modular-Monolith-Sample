using RiverBooks.OrderProcessing.Infrastructure;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Interfaces;

internal interface IOrderAddressCache
{
    Task<Resultable<OrderAddress>> GetByIdAsync(Guid addressId, CancellationToken cancellationToken);

    Task<Resultable> StoreAsync(OrderAddress orderAddress, CancellationToken cancellationToken);
}
