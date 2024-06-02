using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Domain;

internal class OrderItem
{
    public OrderItem(Guid bookId, int quantity, decimal unitPrice, string description)
    {
        BookId = PassOrThrow.IfEmpty(bookId);
        Quantity = PassOrThrow.IfNegative(quantity);
        UnitPrice = PassOrThrow.IfNegative(unitPrice);
        Description = PassOrThrow.IfNullOrWhitespace(description);
    }

    private OrderItem()
    {
        // EF 
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string Description { get; private set; } = string.Empty;

}

