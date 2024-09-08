namespace RiverBooks.OrderProcessing.Contracts;

public class OrderDto
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = [];
}

public record OrderItemDto(Guid BookId,
    int Quantity,
    decimal UnitPrice,
    string Description);
