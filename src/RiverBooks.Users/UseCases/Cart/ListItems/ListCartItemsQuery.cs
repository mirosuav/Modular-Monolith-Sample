using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.CartEndpoints;

namespace RiverBooks.Users.UseCases.Cart.ListItems;

public record ListCartItemsQuery(string EmailAddress) : IRequest<ResultOr<List<CartItemDto>>>;
