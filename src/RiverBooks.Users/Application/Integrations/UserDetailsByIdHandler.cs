using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.UseCases.User.GetById;

namespace RiverBooks.Users.Integrations;

public class UserDetailsByIdHandler(IMediator mediator) : IRequestHandler<UserDetailsByIdQuery, Resultable<UserDetailsDto>>
{
    public async Task<Resultable<UserDetailsDto>> Handle(UserDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(request.UserId);

        var result = await mediator.Send(query, cancellationToken);

        return result.Map(u => new UserDetailsDto(u.UserId, u.EmailAddress));
    }
}
