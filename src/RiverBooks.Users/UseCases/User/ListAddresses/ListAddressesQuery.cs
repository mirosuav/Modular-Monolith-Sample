using Ardalis.Result;
using MediatR;
using RiverBooks.Users.UserEndpoints;

namespace RiverBooks.Users.UseCases.User.ListAddresses;
public record ListAddressesQuery(string EmailAddress) :
  IRequest<Result<List<UserAddressDto>>>;
