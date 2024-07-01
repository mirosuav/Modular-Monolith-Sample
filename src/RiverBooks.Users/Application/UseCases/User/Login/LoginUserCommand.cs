using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Application.UseCases.User.Login;
public record LoginUserCommand(string Email, string Password) : IRequest<Resultable<string>>;
