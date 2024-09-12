using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;

namespace RiverBooks.Users.Application.UseCases.User.GetById;

public class GetUserByIdHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, Resultable<UserDto>>
{
    public async Task<Resultable<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserAsync(request.UserId);

        if (user is null)
        {
            return Error.NotFound("No such user");
        }

        return new UserDto(user.Id, user.Email!);
    }
}

