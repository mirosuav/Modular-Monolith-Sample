using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.User.AddAddress;

public class AddAddressToUserHandler : IRequestHandler<AddAddressToUserCommand, ResultOr>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly ILogger<AddAddressToUserHandler> _logger;

    public AddAddressToUserHandler(IApplicationUserRepository userRepository,
      ILogger<AddAddressToUserHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ResultOr> Handle(AddAddressToUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetUserWithAddressesByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return Error.Unauthorized;
        }

        var addressToAdd = new Address(request.Street1,
                                       request.Street2,
                                       request.City,
                                       request.State,
                                       request.PostalCode,
                                       request.Country);
        var userAddress = user.AddAddress(addressToAdd);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("[UseCase] Added address {address} to user {email} (Total: {total})",
          userAddress.StreetAddress,
          request.EmailAddress,
          user.Addresses.Count);

        return ResultOr.Success();
    }
}
