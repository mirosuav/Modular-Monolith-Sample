namespace RiverBooks.OrderProcessing.Contracts;

public record ListOrdersForUserResponse(List<OrderSummary> Orders);