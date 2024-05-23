using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public record AddItemToCartCommand(Guid BookId, int Quantity, string EmailAddress)
  : IRequest<Result<CartItem>>;
