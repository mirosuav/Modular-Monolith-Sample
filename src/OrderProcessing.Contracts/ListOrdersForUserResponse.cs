using RiverBooks.OrderProcessing.Contracts;

namespace RiverBooks.OrderProcessing.Endpoints;

public class ListOrdersForUserResponse
{
  public List<OrderSummary> Orders { get; set; } = new();
}
