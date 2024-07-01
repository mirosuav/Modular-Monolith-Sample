using RiverBooks.Books.Domain;

namespace RiverBooks.Books.Application;

internal interface IReadOnlyBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<List<Book>> ListAsync();
}
