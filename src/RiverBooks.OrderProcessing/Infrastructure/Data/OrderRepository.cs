using Microsoft.EntityFrameworkCore;
using RiverBooks.OrderProcessing.Application.Interfaces;
using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Infrastructure.Data;

internal class OrderRepository(OrderProcessingDbContext dbContext) : IOrderRepository
{
    public void Add(Order order)
    {
        dbContext.Orders.Add(order);
    }

    public async Task<List<Order>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .Include(o => o.OrderItems)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> DeleteForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<List<Order>> ListForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public void Remove(params Order[] orders)
    {
        dbContext.Orders.RemoveRange(orders);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}