using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Application.UseCases.Cart.AddItem;

public record AddItemToCartCommand(Guid BookId, int Quantity, Guid UserId) : IRequest<ResultOf<bool>>;
