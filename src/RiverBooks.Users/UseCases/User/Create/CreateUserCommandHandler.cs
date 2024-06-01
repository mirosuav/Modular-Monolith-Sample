using MediatR;
using Microsoft.AspNetCore.Identity;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.SharedKernel.Extensions;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.UseCases.User.Create;

public class CreateUserCommandHandler(UserManager<ApplicationUser> userManager,
  IMediator mediator) : IRequestHandler<CreateUserCommand, Resultable>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IMediator _mediator = mediator;

    public async Task<Resultable> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var newUser = new ApplicationUser
        {
            Email = command.Email,
            UserName = command.Email
        };

        var result = await _userManager.CreateAsync(newUser, command.Password);

        if (!result.Succeeded)
        {
            return result.AsErrors();
        }

        // send welcome email
        var sendEmailCommand = new SendEmailCommand
        {
            To = command.Email,
            From = "donotreply@test.com",
            Subject = "Welcome to RiverBooks!",
            Body = "Thank you for registering."
        };

        _ = await _mediator.Send(sendEmailCommand, cancellationToken);

        return Resultable.Success();
    }
}
