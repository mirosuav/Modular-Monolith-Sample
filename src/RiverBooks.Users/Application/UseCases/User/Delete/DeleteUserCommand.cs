using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Application.UseCases.User.Delete;

internal record DeleteUserCommand(Guid UserId) : IRequest<ResultOf>;
