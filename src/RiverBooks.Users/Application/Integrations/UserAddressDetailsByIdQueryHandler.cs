using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Application.Integrations;
public class UserAddressDetailsByIdQueryHandler(IReadOnlyUserStreetAddressRepository addressRepo) :
  IRequestHandler<UserAddressDetailsByIdQuery, Resultable<UserAddressDetails>>
{
    public async Task<Resultable<UserAddressDetails>> Handle(
      UserAddressDetailsByIdQuery request,
      CancellationToken ct)
    {
        var address = await addressRepo.GetById(request.AddressId);

        if (address is null)
            return Error.NotFound("No user address found");

        var details = new UserAddressDetails(
                address.UserId,
                address.Id,
                address.StreetAddress.Street1,
                address.StreetAddress.Street2,
                address.StreetAddress.City,
                address.StreetAddress.State,
                address.StreetAddress.PostalCode,
                address.StreetAddress.Country);

        return details;
    }

}
