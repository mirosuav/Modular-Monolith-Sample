namespace RiverBooks.Users.Contracts;

public record CheckoutRequest(Guid ShippingAddressId, Guid BillingAddressId);
