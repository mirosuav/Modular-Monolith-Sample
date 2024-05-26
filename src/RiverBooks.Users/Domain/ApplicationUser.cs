using Microsoft.AspNetCore.Identity;
using RiverBooks.SharedKernel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiverBooks.Users.Domain;

public class ApplicationUser : IdentityUser, IHaveDomainEvents
{
    public string FullName { get; set; } = string.Empty;

    private readonly List<CartItem> _cartItems = new();
    public IReadOnlyCollection<CartItem> CartItems => _cartItems.AsReadOnly();

    private readonly List<UserStreetAddress> _addresses = new();
    public IReadOnlyCollection<UserStreetAddress> Addresses => _addresses.AsReadOnly();

    private List<DomainEventBase> _domainEvents = new();
    [NotMapped]
    public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
    void IHaveDomainEvents.ClearDomainEvents() => _domainEvents.Clear();



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

    internal UserStreetAddress AddAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        // find existing address and just return it
        var existingAddress = _addresses.SingleOrDefault(a => a.StreetAddress == address);
        if (existingAddress != null)
        {
            return existingAddress;
        }

        var newAddress = new UserStreetAddress(Id, address);
        _addresses.Add(newAddress);


        var domainEvent = new AddressAddedEvent(newAddress);
        RegisterDomainEvent(domainEvent);

        return newAddress;
    }


    internal void ClearCart()
    {
        _cartItems.Clear();
    }
}

