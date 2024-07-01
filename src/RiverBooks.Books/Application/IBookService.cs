using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books.Application;
internal interface IBookService
{
    Task<List<BookDto>> ListBooksAsync();
    Task<Resultable<BookDto>> GetBookByIdAsync(Guid id);
    Task<Resultable> CreateBookAsync(BookDto newBook);
    Task<Resultable> DeleteBookAsync(Guid id);
    Task<Resultable> UpdateBookPriceAsync(Guid bookId, decimal newPrice);
}
