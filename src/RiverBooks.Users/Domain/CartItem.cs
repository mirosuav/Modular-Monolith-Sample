using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Domain;

public class CartItem
{
    public CartItem(Guid bookId, string description, int quantity, decimal unitPrice)
    {
        BookId = Throwable.IfEmpty(bookId);
        Description = Throwable.IfNullOrWhitespace(description);
        Quantity = Throwable.IfNegative(quantity);
        UnitPrice = Throwable.IfNegative(unitPrice);
    }

    public CartItem()
    {
        // EF 
    }
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    internal void UpdateQuantity(int quantity)
    {
        Quantity = Throwable.IfNegative(quantity);
    }

    internal void UpdateDescription(string description)
    {
        Description = Throwable.IfNullOrWhitespace(description);
    }

    internal void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = Throwable.IfNegative(unitPrice);
    }
}

