namespace RiverBooks.Users.Contracts;

public record AddCartItemRequest(Guid BookId, int Quantity);
