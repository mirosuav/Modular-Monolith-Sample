using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;

namespace RiverBooks.Users.Application.UseCases.User.Delete;

internal class DeleteUserCommandHandler(
    IUserRepository userRepository,
    ISender sender)
    : IRequestHandler<DeleteUserCommand, ResultOf>
{
    public async Task<ResultOf> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var result = await userRepository.DeleteUser(request.UserId);

        if (!result)
            return Error.NotFound("User not found");

        // Delete user orders as eventual consistency using domain events

        var deleteUserOrdersCommand = new DeleteUserOrdersCommand(request.UserId);

        return await sender.Send(deleteUserOrdersCommand, cancellationToken);
    }
}
