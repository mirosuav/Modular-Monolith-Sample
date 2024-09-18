using MediatR;

namespace RiverBooks.Users.Contracts;

public record NewUserAddressAddedIntegrationEvent(UserAddressDto UserAddress) : INotification;