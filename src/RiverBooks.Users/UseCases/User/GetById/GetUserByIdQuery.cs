using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.UseCases.User.GetById;
public record GetUserByIdQuery(string UserId) : IRequest<Resultable<UserDTO>>;

