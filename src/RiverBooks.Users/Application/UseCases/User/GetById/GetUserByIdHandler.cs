using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.User.GetById;

public class GetUserByIdHandler(IApplicationUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, Resultable<UserDto>>
{
    public async Task<Resultable<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByIdAsync(request.UserId);

        if (user is null)
        {
            return Error.NotFound("No such user");
        }

        return new UserDto(user.Id, user.Email!);
    }
}

