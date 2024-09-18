using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Application.UseCases.Cart.ListItems;

public record ListCartItemsQuery(Guid UserId) : IRequest<ResultOf<List<CartItemDto>>>;