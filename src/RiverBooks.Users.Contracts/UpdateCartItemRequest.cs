namespace RiverBooks.Users.Contracts;

public record UpdateCartItemRequest(Guid ItemId, int Quantity);