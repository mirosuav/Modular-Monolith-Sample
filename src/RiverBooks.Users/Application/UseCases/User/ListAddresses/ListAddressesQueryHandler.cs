using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Application.UseCases.User.ListAddresses;

public class ListAddressesQueryHandler(IUserRepository userRepository) : IRequestHandler<ListAddressesQuery, ResultOf<List<UserAddressDto>>>
{
    public async Task<ResultOf<List<UserAddressDto>>> Handle(ListAddressesQuery request,
      CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithAddressesAsync(request.UserId);

        if (user is null)
        {
            return Error.NotAuthorized;
        }

        return user!.Addresses!
                    .Select(ua => ua.ToDto())
                    .ToList();
    }
}
