using MediatR;
using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books.Integrations;

internal class BookDetailsQueryHandler :
  IRequestHandler<BookDetailsQuery, ResultOr<BookDetailsResponse>>
{
    private readonly IBookService _bookService;

    public BookDetailsQueryHandler(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<ResultOr<BookDetailsResponse>> Handle(BookDetailsQuery request,
      CancellationToken cancellationToken)
    {
        var book = await _bookService.GetBookByIdAsync(request.BookId);

        if (book is null)
        {
            return Error.CreateNotFound("Book not found");
        }

        var response = new BookDetailsResponse(book.Id, book.Title, book.Author,
                                  book.Price);

        return response;
    }
}
