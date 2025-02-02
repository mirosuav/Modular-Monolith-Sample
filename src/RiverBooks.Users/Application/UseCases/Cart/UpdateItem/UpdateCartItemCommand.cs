using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Application.UseCases.Cart.UpdateItem;

public record UpdateCartItemCommand(Guid ItemId, int Quantity, Guid UserId) : IRequest<ResultOf>;