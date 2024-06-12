using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.User.GetById;

public class GetUserByIdHandler(IApplicationUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, Resultable<UserDTO>>
{
    private readonly IApplicationUserRepository _userRepository = userRepository;

    public async Task<Resultable<UserDTO>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);

        if (user is null)
        {
            return Error.NotFound("No such user");
        }

        return new UserDTO(user.Id, user.Email!);
    }
}

