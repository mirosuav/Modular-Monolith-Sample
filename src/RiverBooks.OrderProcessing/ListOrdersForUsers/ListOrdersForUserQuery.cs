using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.ListOrdersForUser;

public record ListOrdersForUserQuery(string EmailAddress) :
  IRequest<ResultOr<List<OrderSummary>>>;


