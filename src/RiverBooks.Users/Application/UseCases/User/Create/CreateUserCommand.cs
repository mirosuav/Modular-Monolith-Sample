using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.UseCases.User.Create;
public record CreateUserCommand(string Email, string Password) : IRequest<Resultable>;
