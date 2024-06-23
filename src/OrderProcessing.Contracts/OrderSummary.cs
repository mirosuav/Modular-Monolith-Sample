namespace RiverBooks.OrderProcessing.Contracts;

public class OrderSummary
{
    public Guid OrderId { get; set; }
    public string? UserId { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset? DateShipped { get; set; }
    public decimal Total { get; set; }
}
