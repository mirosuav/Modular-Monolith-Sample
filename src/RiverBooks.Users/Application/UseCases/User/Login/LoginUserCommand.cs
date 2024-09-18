using MediatR;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Application.UseCases.User.Login;

public record LoginUserCommand(string Email, string Password) : IRequest<ResultOf<AuthToken>>;