using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Application.UseCases.User.AddAddress;

public record AddAddressToUserCommand(
    Guid UserId,
    string Street1,
    string Street2,
    string City,
    string State,
    string PostalCode,
    string Country)
    : IRequest<ResultOf<Guid>>;