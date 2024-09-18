using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Application.UseCases.User.GetById;

public record GetUserByIdQuery(Guid UserId) : IRequest<ResultOf<UserDto>>;