using RiverBooks.Books.Domain;

namespace RiverBooks.Books.Application;

internal interface IReadOnlyBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<Book?> GetByTitleAndAuthorAsync(string title, string author);
    Task<List<Book>> ListAsync();
}