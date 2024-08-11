using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Domain;

internal class OrderItem
{
    public OrderItem(Guid bookId, int quantity, decimal unitPrice, string description)
    {
        Id = SequentialGuid.NewGuid();
        BookId = PassOrThrowWhen.Empty(bookId);
        Quantity = PassOrThrowWhen.Negative(quantity);
        UnitPrice = PassOrThrowWhen.Negative(unitPrice);
        Description = PassOrThrowWhen.NullOrWhiteSpace(description);
    }

    private OrderItem()
    {
        // EF 
    }

    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string Description { get; private set; } = string.Empty;

}

