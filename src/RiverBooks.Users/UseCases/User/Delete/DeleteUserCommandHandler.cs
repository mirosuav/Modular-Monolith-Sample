﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.SharedKernel.Extensions;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiverBooks.Users.UseCases.User.Delete;

internal class DeleteUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    ISender sender) 
    : IRequestHandler<DeleteUserCommand, Resultable>
{
    private readonly UserManager<ApplicationUser> userManager = userManager;
    private readonly ISender sender = sender;

    public async Task<Resultable> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
            return Error.NotFound("User not found");

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return result.AsErrors();
        }

        // Delete user orders as eventual consistency using domain events

        var deleteUserOrdersCommand = new DeleteUserOrdersCommand(request.UserId);

        return await sender.Send(deleteUserOrdersCommand, cancellationToken);
    }
}