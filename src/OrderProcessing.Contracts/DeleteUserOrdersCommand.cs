using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Contracts;

public record DeleteUserOrdersCommand(Guid UserId) : IRequest<ResultOf<bool>>;
