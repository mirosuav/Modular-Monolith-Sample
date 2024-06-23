using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.CartEndpoints;

namespace RiverBooks.Users.UseCases.Cart.ListItems;

// TODO Use UserId instead of EmailAddress
public record ListCartItemsQuery(string EmailAddress) : IRequest<Resultable<List<CartItemDto>>>;
