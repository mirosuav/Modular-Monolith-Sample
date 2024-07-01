using MediatR;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.User.Login;

public class LoginUserCommandHandler(
    IApplicationUserRepository applicationUserRepository,
    IJwtTokenHandler jwtTokenHandler)
    : IRequestHandler<LoginUserCommand, Resultable<string>>
{
    public async Task<Resultable<string>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await applicationUserRepository.GetUserByEmailAsync(command.Email);

        if (user == null)
        {
            return Error.NotFound();
        }

        var loginSuccessful = user.CheckPassword(command.Password);

        if (!loginSuccessful)
        {
            return Error.NotAuthorized;
        }

        var token = jwtTokenHandler.CreateToken(user.Id.ToString(), user.Email);

        return Resultable.Success(token);
    }
}
