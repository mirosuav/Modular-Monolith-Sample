using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;

namespace RiverBooks.Users.Application.UseCases.User.GetById;

public class GetUserByIdHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, ResultOf<UserDto>>
{
    public async Task<ResultOf<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserAsync(request.UserId);

        if (user is null)
        {
            return Error.NotFound("No such user");
        }

        return new UserDto(user.Id, user.Email!);
    }
}

