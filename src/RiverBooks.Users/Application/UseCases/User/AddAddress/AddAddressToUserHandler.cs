using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Application.UseCases.User.AddAddress;

public class AddAddressToUserHandler(
    IUserRepository userRepository,
    ILogger<AddAddressToUserHandler> logger,
    TimeProvider timeProvider)
    : IRequestHandler<AddAddressToUserCommand, ResultOf<bool>>
{
    public async Task<ResultOf<bool>> Handle(AddAddressToUserCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetUserWithAddressesAsync(request.UserId);

        if (user is null)
        {
            return Error.NotAuthorized;
        }

        var addressToAdd = new Address(request.Street1,
                                       request.Street2,
                                       request.City,
                                       request.State,
                                       request.PostalCode,
                                       request.Country);

        var userAddress = user.AddAddress(addressToAdd, timeProvider);

        await userRepository.SaveChangesAsync(ct);

        logger.LogInformation("Added address {StreetAddress} to user {UserId}",
          userAddress.StreetAddress,
          request.UserId);

        return true;
    }
}
