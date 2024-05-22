using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.User.Create;
public record CreateUserCommand(string Email, string Password) : IRequest<Result>;
