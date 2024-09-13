using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Application.ListOrdersForUsers;

public record ListOrdersForUserQuery(Guid UserId) : IRequest<ResultOf<List<OrderSummary>>>;


