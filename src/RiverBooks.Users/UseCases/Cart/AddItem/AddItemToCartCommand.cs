using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public record AddItemToCartCommand(Guid BookId, int Quantity, string EmailAddress) : IRequest<Resultable>;
