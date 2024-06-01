using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.UseCases.User.GetById;
public record GetUserByIdQuery(Guid UserId) : IRequest<Resultable<UserDTO>>;

