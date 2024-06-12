using MediatR;
using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books.Integrations;

internal class BookDetailsQueryHandler(IBookService bookService) :
  IRequestHandler<BookDetailsQuery, Resultable<BookDetailsResponse>>
{
    private readonly IBookService _bookService = bookService;

    public async Task<Resultable<BookDetailsResponse>> Handle(BookDetailsQuery request,
      CancellationToken cancellationToken)
    {
        var result = await _bookService.GetBookByIdAsync(request.BookId);

        return result.Map(book => new BookDetailsResponse(book.Id, book.Title, book.Author, book.Price));
    }
}
