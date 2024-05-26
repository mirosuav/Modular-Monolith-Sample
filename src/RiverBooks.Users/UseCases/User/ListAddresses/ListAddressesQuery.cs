using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.UseCases.User.ListAddresses;
public record ListAddressesQuery(string EmailAddress) :
  IRequest<ResultOr<List<UserAddressDto>>>;
