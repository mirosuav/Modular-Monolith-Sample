using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Domain;

public class CartItem
{
    public CartItem(Guid bookId, string description, int quantity, decimal unitPrice)
    {
        BookId = ThrowIf.Empty(bookId);
        Description = ThrowIf.NullOrWhitespace(description);
        Quantity = ThrowIf.Negative(quantity);
        UnitPrice = ThrowIf.Negative(unitPrice);
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
        Quantity = ThrowIf.Negative(quantity);
    }

    internal void UpdateDescription(string description)
    {
        Description = ThrowIf.NullOrWhitespace(description);
    }

    internal void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = ThrowIf.Negative(unitPrice);
    }
}

