using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.UseCases.User.Login;
public record LoginUserCommand(string Email, string Password) : IRequest<Resultable<string>>;
