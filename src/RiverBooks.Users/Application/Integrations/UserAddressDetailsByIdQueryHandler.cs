using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Application.Integrations;
public class UserAddressDetailsByIdQueryHandler(IReadOnlyUserStreetAddressRepository addressRepo) :
  IRequestHandler<UserAddressDetailsByIdQuery, ResultOf<UserAddressDto>>
{
    public async Task<ResultOf<UserAddressDto>> Handle(
      UserAddressDetailsByIdQuery request,
      CancellationToken ct)
    {
        var address = await addressRepo.GetById(request.AddressId);

        if (address is null)
            return Error.NotFound("No user address found");

        return address.ToDto();
    }

}
