using MediatR;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Application.UseCases.User.Create;

public class CreateUserCommandHandler(
    IPublisher notificationPublisher,
    IUserRepository userRepository)
    : IRequestHandler<CreateUserCommand, ResultOf>
{
    public async Task<ResultOf> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var userEmail = command.Email.ToLower();

        // Some additional validation for user at db level
        if (await userRepository.GetUserByEmailAsync(userEmail) is not null)
        {
            return Error.Conflict("User.Exists", $"User with email {userEmail} already exists.");
        }

        var newUser = new Domain.User
        {
            Id = SequentialGuid.NewGuid(),
            Email = userEmail,
            FullName = userEmail
        };

        newUser.SetPassword(command.Password);

        userRepository.Add(newUser);

        await userRepository.SaveChangesAsync(cancellationToken);

        // send welcome email
        var sendEmailCommand = new SendEmailCommand
        {
            To = userEmail,
            From = "donotreply@test.com",
            Subject = "Welcome to RiverBooks!",
            Body = "Thank you for registering."
        };

        await notificationPublisher.Publish(sendEmailCommand, cancellationToken);

        return true;
    }
}
