using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Contracts;

public record UserDetailsByIdQuery(string UserId) :
  IRequest<Resultable<UserDetails>>;
