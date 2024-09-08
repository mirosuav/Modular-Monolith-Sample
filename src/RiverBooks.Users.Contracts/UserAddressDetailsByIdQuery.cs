using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Contracts;

public record UserAddressDetailsByIdQuery(Guid AddressId) :
  IRequest<Resultable<UserAddressDto>>;
