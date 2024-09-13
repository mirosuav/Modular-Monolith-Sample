
using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books.Contracts;

public record BookDetailsQuery(Guid BookId) : IRequest<ResultOf<BookDetailsResponse>>;
