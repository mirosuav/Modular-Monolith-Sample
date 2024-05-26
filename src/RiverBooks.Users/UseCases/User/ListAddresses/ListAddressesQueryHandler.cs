using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.User.ListAddresses;

public class ListAddressesQueryHandler : IRequestHandler<ListAddressesQuery, ResultOr<List<UserAddressDto>>>
{
    private readonly IApplicationUserRepository _userRepository;

    public ListAddressesQueryHandler(IApplicationUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ResultOr<List<UserAddressDto>>> Handle(ListAddressesQuery request,
      CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithAddressesByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return Error.Unauthorized;
        }

        return user!.Addresses!
                    .Select(ua => new UserAddressDto(ua.Id, ua.StreetAddress.Street1,
                    ua.StreetAddress.Street2,
                    ua.StreetAddress.City,
                    ua.StreetAddress.State,
                    ua.StreetAddress.PostalCode,
                    ua.StreetAddress.Country))
                    .ToList();
    }
}
