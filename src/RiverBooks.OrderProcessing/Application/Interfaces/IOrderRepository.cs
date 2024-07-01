using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Application.Interfaces;

internal interface IOrderRepository
{
    Task<List<Order>> ListAsync(CancellationToken cancellationToken);
    void Add(Order order);
    void Remove(params Order[] orders);
    Task<List<Order>> ListForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<int> DeleteForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
