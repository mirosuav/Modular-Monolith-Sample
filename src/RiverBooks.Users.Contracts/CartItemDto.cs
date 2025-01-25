namespace RiverBooks.Users.Contracts;

public record CartItemDto(
    Guid Id,
    Guid BookId,
    string Description,
    int Quantity,
    decimal UnitPrice)
{
    public decimal TotalPrice => Quantity * UnitPrice;
}