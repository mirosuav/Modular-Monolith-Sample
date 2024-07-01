using RiverBooks.OrderProcessing.Domain;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Application.Interfaces;

internal interface IOrderAddressCache
{
    Task<Resultable<OrderAddress>> GetByIdAsync(Guid addressId, CancellationToken cancellationToken);

    Task<Resultable> StoreAsync(OrderAddress orderAddress, CancellationToken cancellationToken);
}
