using RiverBooks.OrderProcessing.Infrastructure;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Interfaces;

internal interface IOrderAddressCache
{
    Task<ResultOr<OrderAddress>> GetByIdAsync(Guid addressId);
    Task<ResultOr> StoreAsync(OrderAddress orderAddress);
}
