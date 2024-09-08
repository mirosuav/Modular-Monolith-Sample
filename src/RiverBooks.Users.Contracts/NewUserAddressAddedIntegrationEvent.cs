using MediatR;
using RiverBooks.SharedKernel;

namespace RiverBooks.Users.Contracts;

public record NewUserAddressAddedIntegrationEvent(UserAddressDto UserAddress) : INotification;
