using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

// TODO Use UserID instead of Email
public record AddItemToCartCommand(Guid BookId, int Quantity, string EmailAddress) : IRequest<Resultable>;
