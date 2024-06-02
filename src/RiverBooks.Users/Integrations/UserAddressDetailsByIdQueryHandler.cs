using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Integrations;
public class UserAddressDetailsByIdQueryHandler(IReadOnlyUserStreetAddressRepository addressRepo) :
  IRequestHandler<UserAddressDetailsByIdQuery, Resultable<UserAddressDetails>>
{
    private readonly IReadOnlyUserStreetAddressRepository _addressRepo = addressRepo;

    public async Task<Resultable<UserAddressDetails>> Handle(
      UserAddressDetailsByIdQuery request,
      CancellationToken ct)
    {
        var address = await _addressRepo.GetById(request.AddressId);

        if (address is null)
            return Error.NotFound("No user address found");

        Guid userId = Guid.Parse(address.UserId);

        var details = new UserAddressDetails(userId,
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
