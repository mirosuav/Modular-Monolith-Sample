﻿using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;

namespace RiverBooks.Users.Application.UseCases.User.Delete;

internal class DeleteUserCommandHandler(
    IApplicationUserRepository applicationUserRepository,
    ISender sender)
    : IRequestHandler<DeleteUserCommand, Resultable>
{
    public async Task<Resultable> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var result = await applicationUserRepository.DeleteUser(request.UserId);

        if (!result)
            return Error.NotFound("User not found");

        // Delete user orders as eventual consistency using domain events

        var deleteUserOrdersCommand = new DeleteUserOrdersCommand(request.UserId);

        return await sender.Send(deleteUserOrdersCommand, cancellationToken);
    }
}