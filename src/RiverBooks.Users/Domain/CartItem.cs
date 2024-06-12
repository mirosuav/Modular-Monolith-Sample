using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Domain;
public class CartItem
{
    public CartItem(Guid bookId, string description, int quantity, decimal unitPrice)
    {
        BookId = PassOrThrow.IfEmpty(bookId);
        Description = PassOrThrow.IfNullOrWhitespace(description);
        Quantity = PassOrThrow.IfNegative(quantity);
        UnitPrice = PassOrThrow.IfNegative(unitPrice);
    }

    public CartItem()
    {
        // EF 
    }

    public Guid Id { get; private set; } = SequentialGuid.NewGuid();
    public Guid BookId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    internal void UpdateQuantity(int quantity)
    {
        Quantity = PassOrThrow.IfNegative(quantity);
    }

    internal void UpdateDescription(string description)
    {
        Description = PassOrThrow.IfNullOrWhitespace(description);
    }

    internal void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = PassOrThrow.IfNegative(unitPrice);
    }
}

