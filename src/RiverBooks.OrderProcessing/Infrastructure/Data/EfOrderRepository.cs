using Microsoft.EntityFrameworkCore;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;

namespace RiverBooks.OrderProcessing.Infrastructure.Data;

internal class EfOrderRepository(OrderProcessingDbContext dbContext) : IOrderRepository
{
    private readonly OrderProcessingDbContext _dbContext = dbContext;

    public void Add(Order order)
    {
        _dbContext.Orders.Add(order);
    }

    public async Task<List<Order>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
          .Include(o => o.OrderItems)
          .ToListAsync(cancellationToken);
    }

    public async Task<List<Order>> ListForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
          .Include(o => o.OrderItems)
          .Where(o => o.UserId == userId)
          .ToListAsync(cancellationToken);
    }

    public void Remove(params Order[] orders)
    {
        _dbContext.Orders.RemoveRange(orders);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
