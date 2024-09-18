using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Contracts;

public record UserDetailsByIdQuery(Guid UserId) :
    IRequest<ResultOf<UserDetailsDto>>;