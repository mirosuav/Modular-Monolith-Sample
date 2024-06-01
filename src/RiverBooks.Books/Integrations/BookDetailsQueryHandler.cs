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
