using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.User.ListAddresses;

public class ListAddressesQueryHandler(IApplicationUserRepository userRepository) : IRequestHandler<ListAddressesQuery, Resultable<List<UserAddressDto>>>
{
    public async Task<Resultable<List<UserAddressDto>>> Handle(ListAddressesQuery request,
      CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithAddressesByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return Error.NotAuthorized;
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
