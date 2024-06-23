using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.UseCases.User.Delete;

internal record DeleteUserCommand(string UserId) : IRequest<Resultable>;
