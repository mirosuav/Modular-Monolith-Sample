using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using RiverBooks.SharedKernel.Events;
using RiverBooks.SharedKernel.Extensions;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Domain;

public class User : HaveEvents
{
    private readonly List<UserStreetAddress> _addresses = [];

    private readonly List<CartItem> _cartItems = [];
    public required Guid Id { get; set; }

    [EmailAddress] [MaxLength(50)] public string Email { get; set; } = string.Empty;

    [MaxLength(50)] public string FullName { get; set; } = string.Empty;

    [MaxLength(200)] public string PasswordHash { get; set; } = string.Empty;

    public IReadOnlyCollection<CartItem> CartItems => _cartItems.AsReadOnly();
    public IReadOnlyCollection<UserStreetAddress> Addresses => _addresses.AsReadOnly();

    public void SetPassword(string newPassword)
    {
        var passwordHasher = new PasswordHasher<User>();
        PasswordHash = passwordHasher.HashPassword(this, newPassword);
    }

    public bool CheckPassword(string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(this, PasswordHash, password);
        return result != PasswordVerificationResult.Failed;
    }

    public void AddItemToCart(CartItem cartItem)
    {
        ArgumentNullException.ThrowIfNull(cartItem);

        var existingBook = _cartItems.SingleOrDefault(c => c.BookId == cartItem.BookId);
        if (existingBook != null)
        {
            existingBook.UpdateQuantity(existingBook.Quantity + cartItem.Quantity);
            existingBook.UpdateDescription(cartItem.Description);
            existingBook.UpdateUnitPrice(cartItem.UnitPrice);
            return;
        }

        _cartItems.Add(cartItem);
    }

    internal UserStreetAddress AddAddress(Address address, TimeProvider timeProvider)
    {
        ThrowWhen.Null(address);

        // find existing address and just return it
        var existingAddress = _addresses.SingleOrDefault(a => a.StreetAddress == address);
        if (existingAddress != null) return existingAddress;

        var newAddress = new UserStreetAddress(Id, address);
        _addresses.Add(newAddress);

        var domainEvent = new AddressAddedEvent(newAddress.Id, timeProvider.GetUtcDateTime());

        RegisterEvent(domainEvent);

        return newAddress;
    }

    internal void ClearCart()
    {
        _cartItems.Clear();
    }
}