using MediatR;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Application.UseCases.User.Create;

public class CreateUserCommandHandler(
    IPublisher notificationPublisher,
    IApplicationUserRepository applicationUserRepository)
    : IRequestHandler<CreateUserCommand, Resultable>
{
    public async Task<Resultable> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var userEmail = command.Email.ToLower();

        // Some additional validation for user at db level
        if (await applicationUserRepository.GetUserByEmailAsync(userEmail) is not null)
        {
            return Error.Conflict("User.Exists", $"User with email {userEmail} already exists.");
        }

        var newUser = new ApplicationUser
        {
            Id = SequentialGuid.NewGuid(),
            Email = userEmail,
            FullName = userEmail
        };

        newUser.SetPassword(command.Password);

        applicationUserRepository.Add(newUser);

        await applicationUserRepository.SaveChangesAsync(cancellationToken);

        // send welcome email
        var sendEmailCommand = new SendEmailCommand
        {
            To = userEmail,
            From = "donotreply@test.com",
            Subject = "Welcome to RiverBooks!",
            Body = "Thank you for registering."
        };

        await notificationPublisher.Publish(sendEmailCommand, cancellationToken);

        return Resultable.Success();
    }
}
