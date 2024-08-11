using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Domain;
public class CartItem
{
    public CartItem(Guid bookId, string description, int quantity, decimal unitPrice)
    {
        BookId = PassOrThrowWhen.Empty(bookId);
        Description = PassOrThrowWhen.NullOrWhiteSpace(description);
        Quantity = PassOrThrowWhen.Negative(quantity);
        UnitPrice = PassOrThrowWhen.Negative(unitPrice);
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
        Quantity = PassOrThrowWhen.Negative(quantity);
    }

    internal void UpdateDescription(string description)
    {
        Description = PassOrThrowWhen.NullOrWhiteSpace(description);
    }

    internal void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = PassOrThrowWhen.Negative(unitPrice);
    }
}

