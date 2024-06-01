using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.UseCases.User.GetById;

namespace RiverBooks.Users.Integrations;

public class UserDetailsByIdHandler(IMediator mediator) : IRequestHandler<UserDetailsByIdQuery, Resultable<UserDetails>>
{
    private readonly IMediator _mediator = mediator;

    public async Task<Resultable<UserDetails>> Handle(UserDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(request.UserId);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Map(u => new UserDetails(u.UserId, u.EmailAddress));
    }
}
