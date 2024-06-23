﻿using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Interfaces;

internal interface IOrderRepository
{
    Task<List<Order>> ListAsync(CancellationToken cancellationToken);
    void Add(Order order);
    void Remove(params Order[] orders);
    Task<List<Order>> ListForUserAsync(string userId, CancellationToken cancellationToken);
    Task<int> DeleteForUserAsync(string userId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
