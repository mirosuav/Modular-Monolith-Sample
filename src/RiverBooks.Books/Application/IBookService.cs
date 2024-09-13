using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books.Application;
internal interface IBookService
{
    Task<List<BookDto>> ListBooksAsync();
    Task<ResultOf<BookDto>> GetBookByIdAsync(Guid id);
    Task<ResultOf<bool>> CreateBookAsync(BookDto newBook);
    Task<ResultOf<bool>> DeleteBookAsync(Guid id);
    Task<ResultOf<bool>> UpdateBookPriceAsync(Guid bookId, decimal newPrice);
}
