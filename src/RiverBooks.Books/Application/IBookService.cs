using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books.Application;

internal interface IBookService
{
    Task<List<BookDto>> ListBooksAsync();
    Task<ResultOf<BookDto>> GetBookByIdAsync(Guid id);
    Task<ResultOf> CreateBookAsync(BookDto newBook);
    Task<ResultOf> DeleteBookAsync(Guid id);
    Task<ResultOf> UpdateBookPriceAsync(Guid bookId, decimal newPrice);
}