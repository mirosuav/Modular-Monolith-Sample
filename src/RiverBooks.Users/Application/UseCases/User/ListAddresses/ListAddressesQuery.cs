using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Application.UseCases.User.ListAddresses;
public record ListAddressesQuery(Guid UserId) :
  IRequest<ResultOf<List<UserAddressDto>>>;
