using RiverBooks.OrderProcessing.Contracts;

namespace RiverBooks.OrderProcessing.Endpoints;

public record ListOrdersForUserResponse(List<OrderSummary> Orders);
