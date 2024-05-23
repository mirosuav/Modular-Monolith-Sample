using Ardalis.Result;
using MediatR;
using RiverBooks.OrderProcessing.Contracts;

namespace RiverBooks.OrderProcessing.ListOrdersForUser;

public record ListOrdersForUserQuery(string EmailAddress) : 
  IRequest<Result<List<OrderSummary>>>;


