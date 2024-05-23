using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Contracts;

public class AddressListResponse
{
  public List<UserAddressDto> Addresses { get; set; } = new();
}
