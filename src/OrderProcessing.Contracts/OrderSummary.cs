namespace RiverBooks.OrderProcessing.Contracts;

public class OrderSummary
{
  public Guid OrderId { get; set; }
  public Guid UserId { get; set; }
  public DateTimeOffset DateCreated { get; set; }
  public DateTimeOffset? DateShipped { get; set; }
  public decimal Total { get; set; }
}
