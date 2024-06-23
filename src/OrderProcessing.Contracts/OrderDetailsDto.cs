namespace OrderProcessing.Contracts;

public class OrderDetailsDto
{
    public Guid OrderId { get; set; }
    public string? UserId { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public List<OrderItemDetails> OrderItems { get; set; } = [];
}
