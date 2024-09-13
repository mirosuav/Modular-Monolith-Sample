using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.UseCases.User.GetById;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Application.Integrations;

public class UserDetailsByIdHandler(IMediator mediator) : IRequestHandler<UserDetailsByIdQuery, ResultOf<UserDetailsDto>>
{
    public async Task<ResultOf<UserDetailsDto>> Handle(UserDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(request.UserId);

        var result = await mediator.Send(query, cancellationToken);

        return result.Map(u => new UserDetailsDto(u.UserId, u.EmailAddress));
    }
}
