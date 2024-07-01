namespace OrderProcessing.Contracts;

public class OrderDetailsDto
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public List<OrderItemDetails> OrderItems { get; set; } = [];
}
