using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.UseCases.User.GetById;

namespace RiverBooks.Users.Integrations;

public class UserDetailsByIdHandler : IRequestHandler<UserDetailsByIdQuery, ResultOr<UserDetails>>
{
    private readonly IMediator _mediator;

    public UserDetailsByIdHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ResultOr<UserDetails>> Handle(UserDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(request.UserId);

        var result = await _mediator.Send(query);

        return result.Map(u => new UserDetails(u.UserId, u.EmailAddress));
    }
}
