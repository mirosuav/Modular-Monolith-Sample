using RiverBooks.OrderProcessing.Domain;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Application.Interfaces;

internal interface IOrderAddressCache
{
    Task<ResultOf<OrderAddress>> GetByIdAsync(Guid addressId, CancellationToken cancellationToken);

    Task<ResultOf> StoreAsync(OrderAddress orderAddress, CancellationToken cancellationToken);
}