using MediatR;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.User.Create;

public class CreateUserCommandHandler(
    IPublisher notificationPublisher,
    IApplicationUserRepository applicationUserRepository)
    : IRequestHandler<CreateUserCommand, Resultable>
{
    public async Task<Resultable> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var newUser = new ApplicationUser
        {
            Id = SequentialGuid.NewGuid(),
            Email = command.Email,
            FullName = command.Email
        };

        newUser.SetPassword(command.Password);

        applicationUserRepository.Add(newUser);

        await applicationUserRepository.SaveChangesAsync(cancellationToken);

        // send welcome email
        var sendEmailCommand = new SendEmailCommand
        {
            To = command.Email,
            From = "donotreply@test.com",
            Subject = "Welcome to RiverBooks!",
            Body = "Thank you for registering."
        };

        await notificationPublisher.Publish(sendEmailCommand, cancellationToken);

        return Resultable.Success();
    }
}
