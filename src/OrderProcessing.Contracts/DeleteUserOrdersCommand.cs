using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Contracts;

public record DeleteUserOrdersCommand(string UserId): IRequest<Resultable>;
